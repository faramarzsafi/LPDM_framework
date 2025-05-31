from jmetal.core.solution import PermutationSolution
from thesis.problem.permutation_problem import permutation_problem
import queue


def fixed_size_enqueue(q: queue.Queue, size, item):
    q.put_nowait(item)
    while q.qsize() > size:
        q.get_nowait()


class job_scheduling_instance:
    def __init__(self):
        self.machine_count = 0
        self.jobs_count = 0
        self.costs = []


class job_scheduling(permutation_problem):
    def __init__(self, filename):
        self.number_of_variables = 0
        self.number_of_objectives = 1
        self.instances = []
        self.__read_from_file(filename=filename)
        if len(self.instances) > 0:
            self.instance = self.instances[0]
            self.number_of_variables = self.instance.jobs_count
            self.renew()

    def __add_instance(self, info, data):
        instance = job_scheduling_instance()
        instance.machine_count = len(data)
        for i in range(instance.machine_count):
            instance.costs.append([int(x.strip()) for x in data[i].split()])
        instance.jobs_count = len(instance.costs[0])
        self.instances.append(instance)

    def __read_from_file(self, filename: str):
        if filename is None:
            raise FileNotFoundError("Filename can not be None")

        with open(filename) as file:
            lines = file.readlines()
            data = [line.lstrip() for line in lines if line != ""]

            instance_info_line = ""
            instance_data_lines = []
            i = 0
            while i < len(data):
                if data[i].startswith(
                        "number of jobs, number of machines, initial seed, upper bound and lower bound :"):
                    if len(instance_data_lines) > 0:
                        self.__add_instance(instance_info_line, instance_data_lines)
                    info = data[i + 1]
                    instance_data_lines = []
                    i += 3
                    continue
                instance_data_lines.append(data[i])
                i += 1

    def evaluate(self, solution: PermutationSolution) -> PermutationSolution:
        if self.instance is None:
            raise Exception("No data")

        machines_costs = [0 for i in range(self.instance.jobs_count)]
        jobs_costs = [0 for i in range(self.instance.machine_count)]
        q = queue.Queue()
        for i in range(self.instance.machine_count):
            q.put_nowait(-1)
        for i in range(self.instance.jobs_count + self.instance.machine_count - 1):
            if i < self.instance.jobs_count:
                fixed_size_enqueue(q, self.instance.machine_count, solution.variables[i])
            else:
                fixed_size_enqueue(q, self.instance.machine_count, -1)
            for machine in range(self.instance.machine_count):
                job = q.queue[self.instance.machine_count - machine - 1]
                if job >= 0:
                    new_cost = 0
                    if machines_costs[job] < jobs_costs[machine]:
                        new_cost = jobs_costs[machine]
                    else:
                        new_cost = machines_costs[job]
                    jobs_costs[machine] = machines_costs[job] = new_cost + self.instance.costs[machine][job]

        solution.objectives[0] = machines_costs[self.instance.jobs_count - 1]
        return solution
