import math

from diversity.diversity import diversity, global_number_of_variables


class cheng(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "Cheng"
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

    def compute(self, population) -> float:
        population_count = len(population)
        x = [0.0 for i in range(self.number_of_variables)]
        for i in range(population_count):
            for j in range(self.number_of_variables):
                x[j] += population[i].variables[j] / population_count
        div = [0.0 for i in range(self.number_of_variables)]
        for i in range(population_count):
            for j in range(self.number_of_variables):
                div[j] += abs(population[i].variables[j] - x[j]) / population_count
        div_total = 0.0
        div_max = 0.0
        for i in range(self.number_of_variables):
            div_total += div[i]
            if div_max < div[i]:
                div_max = div[i]
        result = div_total / div_max / self.number_of_variables
        self.add_result(result)
        return result
