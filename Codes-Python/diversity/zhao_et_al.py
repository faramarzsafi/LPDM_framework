from diversity.diversity import diversity, global_number_of_variables
from premutation.premutation import generate_integer_representation


class zhao_et_al(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "Zhao et al."
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

    def compute(self, population) -> float:
        population_count = len(population)
        div_total = 0
        for i in range(population_count):
            for j in range(population_count):
                a = generate_integer_representation(population[i].variables)
                b = generate_integer_representation(population[j].variables)
                if a > b:
                    div_total += a - b
                else:
                    div_total += b - a
        result = div_total
        self.add_result(result)
        return result



