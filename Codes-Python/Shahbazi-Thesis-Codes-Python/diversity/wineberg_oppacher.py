import math

from diversity.diversity import diversity, global_number_of_variables

def hamming_distance(a, b):
    distance = 0
    for i in range(len(a)):
        if a[i] != b[i]:
            distance += 1
    return distance


class wineberg_oppacher(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "Wineberg and Oppacher"
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

    def compute(self, population) -> float:
        population_count = len(population)
        _sum = 0.0
        for i in range(population_count):
            for j in range(0, i - 1):
                _sum += hamming_distance(population[i].variables, population[j].variables)
        result = _sum
        self.add_result(result)
        return result
