from controller import Controller
from neo4j_server import Neo4jServer
from view import View


class Neo4jController(object):
    def __init__(self):
        self.__server = Neo4jServer()
        self.__menu = 'Neo4j menu'
        self.loop = True
        self.start()

    def start(self):
        from data import menu_list
        try:
            while self.loop:
                choice = Controller.make_choice(menu_list[self.__menu].keys(), self.__menu)
                Controller.considering_choice(self, choice, list(menu_list[self.__menu].values()))
        except Exception as e:
            View.show_error(str(e))

    def users_by_tags(self):
        tags = Controller.get_value("Enter tags: ", str)
        res = self.__server.users_by_tags(tags)
        View.print_list("Users: ", res)

    def shortest_path(self):
        username1 = Controller.get_value("Enter first user: ", str)
        username2 = Controller.get_value("Enter second user: ", str)
        res = self.__server.shortest_path(username1, username2)
        View.show_way(res)

    def n_relation(self):
        n = Controller.get_value("Enter n: ", str)
        res = self.__server.n_relation(n)
        View.print_list("Users: ", res)

    def spam_only(self):
        res = self.__server.spam_only()
        View.print_list("Users: ", res)

    def by_tags_without_relations(self):
        tags = Controller.get_value("Enter tags: ", str)
        res = self.__server.by_tags_without_relations(tags)
        View.print_list("Users: ", res)

