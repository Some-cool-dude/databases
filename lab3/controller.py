from inspect import signature
from view import View

from enum import Enum


class Tags(Enum):
    friends = 1,
    family = 2,
    love = 3,
    cars = 4,
    soup = 5

    @classmethod
    def has_member(cls, value):
        return value in Tags._member_names_


class Controller(object):
    @staticmethod
    def make_choice(menu_list: list, name_of_menu: str):
        try:
            View.draw_menu(menu_list, name_of_menu)
            return Controller.get_uint_value("Make your choice: ", len(menu_list))

        except Exception as e:
            View.show_error(str(e))

    @staticmethod
    def considering_choice(controller, choice: int, list_of_func: list):
        try:
            if choice > len(list_of_func) - 1:
                raise Exception("func is not exist")

            desired_func = list_of_func[choice]
            desired_func(controller)
        except Exception as e:
            View.show_error(str(e))

    @staticmethod
    def get_uint_value(msg: str, top_line: int = None):
        while True:
            number = input(msg)
            if number.isdigit():
                number = int(number)
                if top_line is None or 0 <= number < top_line:
                    return number

    @staticmethod
    def get_value(msg: str, type_of_var):
        while True:
            try:
                usr_input = input(msg)
                if type_of_var == str:
                    if len(usr_input) != 0:
                        return type_of_var(usr_input)
                else:
                    return type_of_var(usr_input)
            except Exception as e:
                View.show_error(str(e))

    @staticmethod
    def exit(controller):
        controller.loop = False
