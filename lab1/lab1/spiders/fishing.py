import scrapy


class FishingSpider(scrapy.Spider):
    name = 'fishing'
    allowed_domains = ['fishing-mart.com.ua']
    start_urls = ['https://www.fishing-mart.com.ua/']
    custom_settings = {
        'ITEM_PIPELINES': {
            'lab1.pipelines.Task2Pipeline': 300
        }
    }
    count = 0
    links = []

    def parse(self, response):
        for item in response.xpath('/html/body'):
            text = item.xpath('//a[contains(@class, "product-name")]/text()').extract()
            prices = item.xpath('//span[contains(@class, "price product-price")]/text()').extract()
            images = item.xpath('//a[contains(@class, "product_img_link")]/img/@src').extract()
            self.links += item.xpath("//li/a/@href[starts-with(., 'https://www.fishing-mart.com.ua/')]").extract()
            if len(images) == 0:
                text = []
            yield {
                'text': text,
                'images': images,
                'prices': prices
            }
            self.count += len(images)
            if self.count < 20:
                yield scrapy.Request(
                    response.urljoin(self.links.pop(0)),
                    callback=self.parse
                )
                 
               
                    
                
                