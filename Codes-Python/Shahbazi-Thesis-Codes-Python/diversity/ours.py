from diversity.diversity import diversity, global_number_of_variables


class ours(diversity):
    def __init__(self, number_of_variables=0):
        super().__init__()
        self.name = "LCDM"
        self.clusters_diversities = []
        self.clusters_positional_diversities = []
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables
        self.cluster_location = {}
        self.clusters_count = 0
        for i in range(self.number_of_variables):
            for j in range(self.number_of_variables):
                if i == j:
                    continue
                self.cluster_location[i * self.number_of_variables + j] = self.clusters_count
                self.clusters_count += 1

    def compute(self, population) -> float:
        div_norm = 0
        div_total = 0
        div_max = 0
        d_max = 0
        p_max = 0
        clusters_positional_diversities = [0.0 for i in range(self.clusters_count)]
        clusters_diversities = [0.0 for i in range(self.clusters_count)]
        cluster_distances = [0.0 for i in range(self.clusters_count)]
        clusters = [0.0 for i in range(self.clusters_count)]
        for i in range(len(population)):
            clusters[self.cluster_location[population[i].variables[0] * self.number_of_variables + population[i].variables[1]]] += 1
        for i in range(self.clusters_count):
            if p_max < clusters[i]:
                p_max = clusters[i]

        m = [0.0 for i in range(self.clusters_count)]  # M
        m_count = 0
        for i in range(self.clusters_count):
            if clusters[i] == p_max:
                m[m_count] = i
                m_count += 1

        for i in range(self.clusters_count):
            d = 0
            for j in range(m_count):
                d += abs(i - m[j])
            cluster_distances[i] = d
            if d_max < d:
                d_max = d

        for i in range(self.clusters_count):
            clusters_diversities[i] = clusters[i] / p_max
            clusters_positional_diversities[i] = clusters_diversities[i] / p_max * (1 + cluster_distances[i] / d_max)
            if div_max < clusters_positional_diversities[i]:
                div_max = clusters_positional_diversities[i]
            div_total += clusters_positional_diversities[i] / self.clusters_count

        result = div_total / self.clusters_count
        self.clusters_diversities.append(clusters_diversities)
        self.clusters_positional_diversities.append(clusters_positional_diversities)
        self.add_result(result)
        return result

    def get_total_diversity(self) -> float:
        clusters_positional_diversities = [0.0 for i in range(self.clusters_count)]
        div_total = 0
        div_max = 0
        for i in range(self.clusters_count):
            for j in range(len(self.clusters_positional_diversities)):
                clusters_positional_diversities[i] += self.clusters_positional_diversities[j][i]
            clusters_positional_diversities[i] /= len(self.clusters_positional_diversities)
            div = clusters_positional_diversities[i] / self.clusters_count
            div_total += div
            if div_max < div:
                div_max = div

        result = div_total / self.clusters_count / div_max
        return result

    def add_result(self, result):
        self.results.append(result)
