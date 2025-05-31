import math

from diversity.diversity import diversity, global_number_of_variables


class ursem(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "Ursem"
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

    def compute(self, population) -> float:
        population_count = len(population)
        _l = math.sqrt(self.number_of_variables * self.number_of_variables * self.number_of_variables)
        sm = [0.0 for i in range(self.number_of_variables)]
        for i in range(population_count):
            for j in range(self.number_of_variables):
                sm[j] += population[i].variables[j]
        for i in range(self.number_of_variables):
            sm[i] /= population_count
        div_total = 0
        for i in range(population_count):
            t = 0
            for j in range(self.number_of_variables):
                x = population[i].variables[j] - sm[j]
                t += x * x
            div_total += math.sqrt(t)
        result = div_total / _l / self.number_of_variables
        self.add_result(result)
        return result
