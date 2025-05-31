import os
import shutil

from diversity.ours import ours
from premutation.premutation import generate_integer_representation

excel_path = "Template.xlsx"
result_output_path = ""


def print_solutions(iteration_no, solution_list):
    s = "Iteration: [{:02d}] ".format(iteration_no)
    print(s)
    for i in range(len(solution_list)):
        solution = solution_list[i]
        solution_variables = ",".join(str(x) for x in solution.variables)

        print("{:10d}: [{}]".format(generate_integer_representation(solution.variables), solution_variables))


def print_problem_diversities(problem, filename=None, output_path=None):
    if output_path is None:
        output_path = result_output_path
    diversities_count = len(problem.diversities)
    iteration_count = 0
    s = ""
    for i in range(diversities_count):
        if s != "":
            s += ","
        else:
            iteration_count = len(problem.diversities[i].results)
        s += problem.diversities[i].get_name()
    s += "\n"
    for i in range(iteration_count):
        row = ""
        for j in range(diversities_count):
            if row != "":
                row += ","
            row += str(problem.diversities[j].results[i])
        s += row + "\n"
    s += "\n"
    if filename is not None:
        path = os.path.join(output_path, filename)
        if not os.path.exists(path):
            os.makedirs(path)
        shutil.copyfile(excel_path, os.path.join(path, "chart-" + filename + ".xlsx"))
        with open(os.path.join(path, "data-" + filename + ".csv"), "w") as file:
            print(s, file=file)
    else:
        print(s)


def print_problem_costs(problem, filename=None, output_path=None):
    if output_path is None:
        output_path = result_output_path
    s = "Best Cost, Avg Cost"
    for i in range(len(problem.best_costs)):
        s += "\n{},{}".format(problem.best_costs[i], problem.costs_avg[i])
    s += "\n"
    if filename is not None:
        path = os.path.join(output_path, filename)
        if not os.path.exists(path):
            os.makedirs(path)
        with open(os.path.join(path, "costs-" + filename + ".csv"), "w") as file:
            print(s, file=file)
    else:
        print(s)


def print_population(solution_list, filename=None, output_path=None):
    if output_path is None:
        output_path = result_output_path
    s = ""
    for i in range(len(solution_list)):
        solution = solution_list[i]
        solution_variables = ",".join(str(x) for x in solution.variables)

        s += "{}; [{}]\n".format(generate_integer_representation(solution.variables), solution_variables)
    s += "\n"
    if filename is not None:
        path = os.path.join(output_path, 'pop', filename)
        if not os.path.exists(path):
            os.makedirs(path)
        with open(os.path.join(path, "pop-" + filename + ".csv"), "w") as file:
            print(s, file=file)
    else:
        print(s)


def print_problem_ours(problem, filename=None, output_path=None):
    if output_path is None:
        output_path = result_output_path
    diversities_count = len(problem.diversities)
    diversity = None
    for i in range(diversities_count):
        if type(problem.diversities[i]) == ours:
            diversity = problem.diversities[i]
    if diversity is None:
        print("Our diversity not used.")
        return
    s = "cluster diversity\n"
    for i in range(len(diversity.clusters_diversities)):
        s += ",".join(str(x) for x in diversity.clusters_diversities[i]) + "\n"
    s += "\ncluster positional-diversity\n"
    for i in range(len(diversity.clusters_positional_diversities)):
        s += ",".join(str(x) for x in diversity.clusters_positional_diversities[i]) + "\n"
    s += "\ntotal diversity: {}\n".format(diversity.get_total_diversity())

    if filename is not None:
        path = os.path.join(output_path, filename)
        if not os.path.exists(path):
            os.makedirs(path)
        with open(os.path.join(path, "ours-" + filename + ".csv"), "w") as file:
            print(s, file=file)
    else:
        print(s)


def print_results(algorithm, problem, result, filename=None, output_path=None):
    if output_path is None:
        output_path = result_output_path
    s = ""
    s += "Algorithm: {}\n".format(algorithm.get_name())
    s += "Mutation: {}({})\n".format(algorithm.mutation_operator.get_name(), algorithm.mutation_operator.probability)
    s += "Crossover: {}({})\n".format(algorithm.crossover_operator.get_name(), algorithm.crossover_operator.probability)
    s += "Problem: {}\n".format(problem.get_name())
    s += "Solution: {}\n".format(result.variables)
    s += "Fitness: {}\n".format(result.objectives[0])
    s += "Computing time: {}\n".format(algorithm.total_computing_time)
    if filename is not None:
        path = os.path.join(output_path, filename)
        if not os.path.exists(path):
            os.makedirs(path)
        with open(os.path.join(path, "result-" + filename + ".txt"), "w") as file:
            print(s, file=file)
    print(s)
