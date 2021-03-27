const Promise = require('bluebird');
const redis = Promise.promisifyAll(require('redis'));

const client = redis.createClient({ host: '0.0.0.0', db: 1 });
client.on("error", err => {
    console.error(err);
    client.end(true);
});

const publisher = client.duplicate();
const subscriber = client.duplicate();

const sleep = (milliseconds) => {  
    return new Promise(resolve => setTimeout(resolve, milliseconds));  
 } 

const spamCheck = async () => {
    await sleep(Math.random()*1000);
    return Math.round(Math.random());
}

subscriber.subscribe('send');

subscriber.on('message', async (channel, mess) => {
    const json = await client.lpopAsync('queue');
    const { sender, message, receiver, username } = JSON.parse(json);
    await client.decrAsync(`${sender} Queued`);
    await client.incrAsync(`${sender} Spam check`);
    let res = await spamCheck();
    if (res) {
        await client.decrAsync(`${sender} Spam check`);
        await client.incrAsync(`${sender} Sended`);
        const score = await client.getAsync(`${sender} Sended`);
        await client.zaddAsync('senders', ...[score, username]);
        await client.rpush(receiver, `${username}>>>\n${message}\n<<<`);
        await client.incrAsync(`${receiver} Received`);
    } else {
        await client.decrAsync(`${sender} Spam check`);
        await client.incrAsync(`${sender} Blocked`);
        const score = await client.getAsync(`${sender} Blocked`);
        await client.zaddAsync('spamers', ...[score, username]);
    }
});