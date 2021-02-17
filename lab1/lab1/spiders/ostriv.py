import scrapy


class OstrivSpider(scrapy.Spider):
    name = 'ostriv'
    allowed_domains = ['ostriv.in.ua']
    start_urls = ['http://www.ostriv.in.ua/']
    custom_settings = {
        'ITEM_PIPELINES': {
            'lab1.pipelines.Task1Pipeline': 300
        }
    }
    link_count = 0
    count = 0

    def parse(self, response):
        for item in response.xpath('/html/body'):
            text = item.xpath('//*[not(self::script)]/text()').extract()
            images = item.xpath('//img/@src').extract()
            links = item.xpath("//a[not(@href='javascript:void(0);')]/@href").extract()
            yield {
                'text': text,
                'images': images,
                'page': response.url,
                'links': links
            }
            for link in links:
                if self.count < 20:
                    yield scrapy.Request(
                        response.urljoin(link),
                        callback=self.parse
                    )
                else:
                    return
                self.count += 1
                    
                
                