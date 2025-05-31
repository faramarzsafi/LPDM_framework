import math

from diversity.diversity import diversity, global_number_of_variables


class pop_lim:
    def __init__(self, number_of_variables):
        self.number_of_variables = number_of_variables
        self.max = -1.0
        self.min = number_of_variables + 1


class osuna_enciso_et_al(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables
        self.name = "Osuna Enciso et al."

    def compute(self, population) -> float:
        v_lim = 1.0
        population_count = len(population)
        for i in range(self.number_of_variables):
            v_lim *= self.number_of_variables
        pop_lims = [pop_lim(self.number_of_variables) for i in range(self.number_of_variables)]
        for i in range(population_count):
            for j in range(self.number_of_variables):
                if pop_lims[j].max < population[i].variables[j]:
                    pop_lims[j].max = population[i].variables[j]
                if pop_lims[j].min > population[i].variables[j]:
                    pop_lims[j].min = population[i].variables[j]
        v_pop = 1.0
        for i in range(self.number_of_variables):
            lim = pop_lims[i].max - pop_lims[i].min
            q = (lim + 1.0) / 4
            v_pop *= 2 * q
        result = math.sqrt(math.sqrt(v_pop) / math.sqrt(v_lim))
        self.add_result(result)
        return result
