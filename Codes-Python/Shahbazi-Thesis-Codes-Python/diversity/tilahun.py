import math

from diversity.diversity import diversity, global_number_of_variables


class tilahun(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "Tilahun"
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

    def compute(self, population) -> float:
        population_count = len(population)
        x_c = [0.0 for i in range(self.number_of_variables)]
        for i in range(population_count):
            for j in range(self.number_of_variables):
                x_c[j] += population[i].variables[j] / population_count
        div_total = 0.0
        for i in range(population_count):
            _sum = 0.0
            for j in range(self.number_of_variables):
                _sum += (population[i].variables[j] - x_c[j]) * (population[i].variables[j] - x_c[j])
            div_total += math.sqrt(_sum)
        _l = math.sqrt(self.number_of_variables ** 3)
        result = div_total / (_l * self.number_of_variables)
        self.add_result(result)
        return result

