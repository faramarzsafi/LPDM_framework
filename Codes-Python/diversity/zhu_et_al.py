import math

from diversity.diversity import diversity, global_number_of_variables


class edge:
    def __init__(self, a, b):
        self.a = a
        self.b = b


class zhu_et_al(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "Zhu et al."
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

    def compute(self, population) -> float:
        population_count = len(population)
        _sum = 0.0
        for i in range(population_count - 1):
            for j in range(i+1, population_count):
                edge_a = [None for k in range(self.number_of_variables - 1)]
                edge_b = [None for k in range(self.number_of_variables - 1)]
                for k in range(self.number_of_variables - 1):
                    edge_a[k] = edge(population[i].variables[k], population[i].variables[k + 1])
                    edge_b[k] = edge(population[j].variables[k], population[j].variables[k + 1])
                for k in range(self.number_of_variables - 1):
                    found_edge = False
                    for m in range(self.number_of_variables - 1):
                        if edge_a[k].a == edge_b[m].a and edge_a[k].b == edge_b[m].b:
                            found_edge = True
                            break
                    if not found_edge:
                        _sum += 1
        result = _sum
        self.add_result(result)
        return result
