import math

from diversity.diversity import diversity, global_number_of_variables


class shi(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "Shi"
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

    def compute(self, population) -> float:
        population_count = len(population)
        div_total = 0
        for i in range(population_count):
            for j in range(population_count):
                _sum = 0.0
                for k in range(self.number_of_variables):
                    _sum += (population[i].variables[k] - population[j].variables[k]) ** 2
                div_total += math.sqrt(_sum)
        _l = math.sqrt(self.number_of_variables * self.number_of_variables * self.number_of_variables)
        result = 2 * div_total / population_count / (population_count - 1) / _l
        self.add_result(result)
        return result
