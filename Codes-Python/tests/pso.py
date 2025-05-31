import datetime
import random
import math
import sys

import matplotlib.pyplot as plt

from jmetal.core.solution import PermutationSolution
from output import print_problem_diversities, print_problem_ours, print_problem_costs
from tests.tsp_problem import tsp_problem

from tests.util import City, read_cities, write_cities_and_return_them, generate_cities, path_cost


class Particle:
    def __init__(self, route, cost=None):
        self.route = route
        self.pbest = route
        self.current_cost = cost if cost else self.path_cost()
        self.pbest_cost = cost if cost else self.path_cost()
        self.velocity = []

    def clear_velocity(self):
        self.velocity.clear()

    def update_costs_and_pbest(self):
        self.current_cost = self.path_cost()
        if self.current_cost < self.pbest_cost:
            self.pbest = self.route
            self.pbest_cost = self.current_cost

    def path_cost(self):
        return path_cost(self.route)


class PSO:

    def __init__(self, iterations, population_size, gbest_probability=1.0, pbest_probability=1.0, cities=None):
        self.cities = cities
        self.gbest = None
        self.gcost_iter = []
        self.iterations = iterations
        self.population_size = population_size
        self.particles = []
        self.gbest_probability = gbest_probability
        self.pbest_probability = pbest_probability

        solutions = self.initial_population()
        self.particles = [Particle(route=solution) for solution in solutions]
        self.problem = tsp_problem(len(cities))

    def random_route(self):
        return random.sample(self.cities, len(self.cities))

    def initial_population(self):
        random_population = [self.random_route() for _ in range(self.population_size - 1)]
        greedy_population = [self.greedy_route(0)]
        return [*random_population, *greedy_population]
        # return [*random_population]

    def greedy_route(self, start_index):
        unvisited = self.cities[:]
        del unvisited[start_index]
        route = [self.cities[start_index]]
        while len(unvisited):
            index, nearest_city = min(enumerate(unvisited), key=lambda item: item[1].distance(route[-1]))
            route.append(nearest_city)
            del unvisited[index]
        return route

    def run(self):
        self.gbest = min(self.particles, key=lambda p: p.pbest_cost)
        print(f"initial cost is {self.gbest.pbest_cost}")
        # plt.ion()
        # plt.draw()
        for t in range(self.iterations):
            self.gbest = min(self.particles, key=lambda p: p.pbest_cost)
            if t % 20 == 0:
                # plt.figure(0)
                # plt.plot(pso.gcost_iter, 'g')
                # plt.ylabel('Distance')
                # plt.xlabel('Generation')
                # fig = plt.figure(0)
                # fig.suptitle('pso iter')
                x_list, y_list = [], []
                for city in self.gbest.pbest:
                    x_list.append(city.x)
                    y_list.append(city.y)
                x_list.append(self.gbest.pbest[0].x)
                y_list.append(self.gbest.pbest[0].y)
                # fig = plt.figure(1)
                # fig.clear()
                # fig.suptitle(f'pso TSP iter {t}')
                #
                # plt.plot(x_list, y_list, 'ro')
                # plt.plot(x_list, y_list, 'g')
                # plt.draw()
                # plt.pause(.001)
            self.gcost_iter.append(self.gbest.pbest_cost)

            for particle in self.particles:
                particle.clear_velocity()
                temp_velocity = []
                gbest = self.gbest.pbest[:]
                new_route = particle.route[:]

                for i in range(len(self.cities)):
                    if new_route[i] != particle.pbest[i]:
                        swap = (i, particle.pbest.index(new_route[i]), self.pbest_probability)
                        temp_velocity.append(swap)
                        new_route[swap[0]], new_route[swap[1]] = \
                            new_route[swap[1]], new_route[swap[0]]

                for i in range(len(self.cities)):
                    if new_route[i] != gbest[i]:
                        swap = (i, gbest.index(new_route[i]), self.gbest_probability)
                        temp_velocity.append(swap)
                        gbest[swap[0]], gbest[swap[1]] = gbest[swap[1]], gbest[swap[0]]

                particle.velocity = temp_velocity

                for swap in temp_velocity:
                    if random.random() <= swap[2]:
                        new_route[swap[0]], new_route[swap[1]] = \
                            new_route[swap[1]], new_route[swap[0]]

                particle.route = new_route
                particle.update_costs_and_pbest()

            solutions = []
            the_best_cost = sys.maxsize
            costs_sum = 0
            for particle in self.particles:
                solution = PermutationSolution(number_of_variables=self.problem.number_of_variables, number_of_objectives=1, number_of_constraints=0)
                i = 0
                for r in particle.route:
                    solution.variables[i] = r.n
                    i += 1
                solutions.append(solution)
                costs_sum += particle.current_cost
                if the_best_cost > particle.current_cost:
                    the_best_cost = particle.current_cost
            self.problem.compute_diversities(solutions, the_best_cost, costs_sum/self.problem.number_of_variables)


def run_pso():
    cities = read_cities(100)
    pso = PSO(iterations=120, population_size=100, pbest_probability=0.8, gbest_probability=0.02, cities=cities)
    pso.run()
    print(f'cost: {pso.gbest.pbest_cost}\t| gbest: {pso.gbest.pbest}')
    #
    # x_list, y_list = [], []
    # for city in pso.gbest.pbest:
    #     x_list.append(city.x)
    #     y_list.append(city.y)
    # x_list.append(pso.gbest.pbest[0].x)
    # y_list.append(pso.gbest.pbest[0].y)
    # fig = plt.figure(1)
    # fig.suptitle('pso TSP')
    #
    # plt.plot(x_list, y_list, 'ro')
    # plt.plot(x_list, y_list)
    # plt.show(block=True)
    problem = pso.problem
    now = datetime.datetime.now()
    time_name = now.strftime("%Y%m%d-%H%M%S")+'-pso'
    # print_results(algorithm, problem, result, filename=time_name)
    print_problem_diversities(problem, filename=time_name)
    print_problem_costs(problem, filename=time_name)
    # print_problem_ours(problem, filename=time_name)

