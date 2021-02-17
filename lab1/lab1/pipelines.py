# Define your item pipelines here
#
# Don't forget to add your pipeline to the ITEM_PIPELINES setting
# See: https://docs.scrapy.org/en/latest/topics/item-pipeline.html


# useful for handling different item types with a single interface
from itemadapter import ItemAdapter
import xml.etree.cElementTree as ET
import re


class Task1Pipeline:
    def open_spider(self, spider):
        self.root = ET.Element("data")
        self.regex = re.compile(r'^[ \t\r\n]*$')
        self.links = []

    def close_spider(self, spider):
        for link in self.links:
            ET.SubElement(self.root, "link").text = link
        tree = ET.ElementTree(self.root)
        tree.write("task1.xml", encoding='utf-8')

    def process_item(self, item, spider):
        items = ItemAdapter(item).asdict()
        page = ET.SubElement(self.root, "page", url=items["page"])
        filtered = [i for i in items["text"] if not self.regex.match(i)]
        ET.SubElement(page, "fragment", type="text").text = ''.join(filtered)
        filtered = [i for i in items["images"] if not self.regex.match(i)]
        ET.SubElement(page, "fragment", type="image").text = ' '.join(filtered)
        self.links += items["links"]
        return item

class Task2Pipeline:
    text = []
    images = []
    prices = []

    def open_spider(self, spider):
        self.root = ET.Element("data")

    def close_spider(self, spider):
        tree = ET.ElementTree(self.root)
        tree.write("task2.xml", encoding='utf-8')

    def process_item(self, item, spider):
        items = ItemAdapter(item).asdict()
        self.text += items['text']
        self.images += items['images']
        self.prices += items['prices']
        if len(self.images) >= 20 :
            for i in range(20):
                elem = ET.SubElement(self.root, "item")
                ET.SubElement(elem, "description").text = self.text[i]
                ET.SubElement(elem, "price").text = self.prices[i]
                ET.SubElement(elem, "image").text = self.images[i]
        return item
