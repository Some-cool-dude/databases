
from controller import Controller
from generator import GeneratorController
from neo4j_controller import Neo4jController
from view import View
from faker import Faker
import random


def generator():

    fake = Faker()
    users_count = 1000
    users = [fake.profile(fields=['username'], sex=None)['username'] for u in range(users_count)]
    threads = []
    try:

        for i in range(users_count):
            threads.append(GeneratorController(users[i], users, users_count, random.randint(1, 2)))
        for thread in threads:
            thread.start()

    except Exception as e:
        View.show_error(str(e))
    finally:
        for thread in threads:
            if thread.is_alive():
                thread.stop()


if __name__ == "__main__":
    choice = Controller.make_choice(["neo4j", "genearte"], "Welcome")
    if choice == 0:
        Neo4jController()
    elif choice == 1:
        generator()
