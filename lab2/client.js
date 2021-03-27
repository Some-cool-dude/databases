const question = require('prompt-sync')();
const crypto = require('crypto');
const Promise = require('bluebird');
const redis = Promise.promisifyAll(require('redis'));

let username = '';
let type = '';
let hash = '';

const secret = 'sjh37648^$';

const client = redis.createClient({ host: '0.0.0.0', db: 1 });


try {
    const publisher = client.duplicate();
    const subscriber = client.duplicate();

    const getHash = str => {
        return crypto.createHash('sha256', secret).update(str).digest('hex');
    }

    const send = async () => {
        console.clear();
        const receiver = question('Send to: ');
        if (!receiver) {
            console.log('Cannot be empty');
            return;
        } else if (receiver === username) {
            console.log(`You can't send massage to yourself`);
            return;
        }
        const text = question('> ');
        if (!text) {
            console.log('Cannot be empty');
            return;
        }
        await client.incrAsync(`${hash} Created`);
        const rec = await client.hgetallAsync(receiver);
        await client.rpushAsync('queue', JSON.stringify({
            username,
            sender: hash,
            message: text,
            receiver: rec.messages
        }));
        publisher.publish('send', '');
        await client.decrAsync(`${hash} Created`);
        await client.incrAsync(`${hash} Queued`);
        console.clear();
    }

    const show = async () => {
        console.clear();
        const messages = await client.lrangeAsync(hash, 0, -1);
        messages.forEach(message => console.log(message));
    }

    const statistic = async () => {
        console.clear();
        let value = await client.getAsync(`${hash} Created`);
        console.log(`Created: ${value}`);
        value = await client.getAsync(`${hash} Queued`);
        console.log(`Queued: ${value}`);
        value = await client.getAsync(`${hash} Spam check`);
        console.log(`Spam check: ${value}`);
        value = await client.getAsync(`${hash} Blocked`);
        console.log(`Blocked: ${value}`);
        value = await client.getAsync(`${hash} Sended`)
        console.log(`Sended: ${value}`);
        value = await client.getAsync(`${hash} Received`);
        console.log(`Received: ${value}`);
    }

    const online = async () => {
        console.clear();
        console.log('Online:');
        const users = await client.smembersAsync('online')
        users.forEach(user => console.log(user));
    }

    const senders = async () => {
        console.clear();
        const n = question('Quantity:');
        console.log('Senders:');
        const s = await client.zrevrangebyscoreAsync(["senders", Infinity, n, "WITHSCORES"]);
        s.forEach(item => console.log(item));
    }

    const spamers = async () => {
        console.clear();
        let n = question('Quantity:');
        n = parseInt(n);
        if (!n) return;
        console.log('Spamers:');
        const s = await client.zrevrangebyscoreAsync(["spamers", Infinity, n, "WITHSCORES"]);
        s.forEach(item => console.log(item));
    }

    const generate = async () => {
        console.clear();
        console.log('Generate started');
        const users = await client.smembersAsync('online')
        await Promise.all(users.map(async user => {
            if (user !== username) {
                for (let n = 0; n < 1000; n++) {
                    await client.incrAsync(`${hash} Created`);
                    const rec = await client.hgetallAsync(user);
                    let text = '';
                    for (let i = 0; i < Math.random() * 1000; i++) {
                        text += 'test';
                    }
                    await client.rpushAsync('queue', JSON.stringify({
                    username,
                    sender: hash,
                    message: text,
                    receiver: rec.messages
                    }));
                    publisher.publish('send', '');
                    await client.decrAsync(`${hash} Created`);
                    await client.incrAsync(`${hash} Queued`);
                }
            }
        }));
        console.log('Generate finished');
    }

    const menu = async () => {
        while (true) {
            console.log('> Quit (/q)');
            console.log('> Send message (/s)');
            console.log('> Show messages (/m)');
            console.log('> Statistic (/n)');
            console.log('> Generate (/g)');
            if (type === 'admin') {
                console.log('> Online (/o)');
                console.log('> Senders (/e)');
                console.log('> Spamers (/b)');
            }
            const option = question('Option:');
            switch (option) {
                case '/q': 
                    return;
                case '/s':
                    await send();
                    break;
                case '/m':
                    await show();
                    break;
                case '/n':
                    await statistic();
                    break;
                case '/g':
                    await generate();
                    break;
                case '/o':
                    if (type === 'admin') await online();
                    break;
                case '/e':
                    if (type === 'admin') await senders();
                    break;
                case '/b':
                    if (type === 'admin') await spamers();
                    break;
            }
        }
    }

    const start = async () => {
        username = question('Enter username:');
        if (!username) {
            console.log('Cannot be empty');
            await client.sremAsync('online', username);
            client.end(true);
            return;
        }
        await client.saddAsync('online', username);
        user = await client.hgetallAsync(username);
        if (!user) {
            hash = getHash(username);
            await client.hmsetAsync(username, ...['type', 'user'], ...['messages', hash]);
            type = 'user';
            messages = hash;
            await client.setAsync(`${hash} Created`, 0);
            await client.setAsync(`${hash} Queued`, 0);
            await client.setAsync(`${hash} Spam check`, 0);
            await client.setAsync(`${hash} Blocked`, 0);
            await client.setAsync(`${hash} Sended`, 0);
            await client.setAsync(`${hash} Received`, 0);
        } else {
            ({ type, messages: hash } = user);
        }

        await menu();
        await client.sremAsync('online', username);
        client.end(true);
        process.exit(0);
    }

    start();
} catch (e) {
    console.error(e);
    (async () => await client.sremAsync('online', username))()
    client.end(true);
    process.exit(0);
}