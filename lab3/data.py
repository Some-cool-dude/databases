from neo4j_controller import Neo4jController
from controller import Controller, Tags
from neo4j_server import Neo4jServer

menu_list = {
    'Neo4j menu': {
        'Users by tags': Neo4jController.users_by_tags,
        'N relation': Neo4jController.n_relation,
        'Shortest path': Neo4jController.shortest_path,
        'Spam only': Neo4jController.spam_only,
        'Users by tags without relations': Neo4jController.by_tags_without_relations,
        'Exit': Controller.exit,
    }
}

neo4j = Neo4jServer()