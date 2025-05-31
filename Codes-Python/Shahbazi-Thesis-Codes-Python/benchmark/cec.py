import math

from jmetal.core.solution import PermutationSolution, FloatSolution


def map_xy(x, x_min, x_max, y_min, y_max):
    offset = y_min
    ratio = (y_max - y_min) / (x_max - x_min)
    return ratio * (x - x_min) + offset


def ackley_fcn_map(x, x_min, x_max):
    y_min = -35
    y_max = +36
    return map_xy(x, x_min, x_max, y_min, y_max)


def ackley_fcn(solution: PermutationSolution) -> float:
    a = 20.0
    b = 0.2
    c = 2.0 * math.pi
    sum_squre = 0
    sum_cx = 0
    variables_count = len(solution.variables)
    for i in range(variables_count):
        x = ackley_fcn_map(solution.variables[i], 0, variables_count-1)
        sum_squre += x * x
        sum_cx += math.cos(c * x)
    try:
        result = -a * math.exp(-b * math.sqrt(1 / variables_count * sum_squre)) - math.exp(
            1 / variables_count * sum_cx) + a + math.exp(1)
    except Exception as e:
        print(str(e))
    return result

def ackley_n4_fcn_map(x, x_min, x_max):
    y_min = -35
    y_max = +36
    return map_xy(x, x_min, x_max, y_min, y_max)

def ackley_n4_fcn(solution: PermutationSolution) -> float:
    _sum = 0
    variables_count = len(solution.variables)
    for i in range(variables_count-1):
        x_i = ackley_n4_fcn_map(solution.variables[i], 0, variables_count - 1)
        x_ii = ackley_n4_fcn_map(solution.variables[i+1], 0, variables_count - 1)
        _sum = math.exp(-0.2) * math.sqrt(x_i * x_i + x_ii * x_ii) + 3 * (math.cos(2 * x_i) + math.sin(2 * x_ii))
    return _sum

def ackley_n4_fcn_real(solution: FloatSolution) -> float:
    _sum = 0
    variables_count = len(solution.variables)
    for i in range(variables_count-1):
        x_i = solution.variables[i]
        x_ii = solution.variables[i+1]
        _sum = math.exp(-0.2) * math.sqrt(x_i * x_i + x_ii * x_ii) + 3 * (math.cos(2 * x_i) + math.sin(2 * x_ii))
    return _sum

def ackley_fcn_real(solution: FloatSolution) -> float:
    a = 20.0
    b = 0.2
    c = 2.0 * math.pi
    sum_squre = 0
    sum_cx = 0
    variables_count = len(solution.variables)
    for i in range(variables_count):
        x = solution.variables[i]
        sum_squre += x * x
        sum_cx += math.cos(c * x)
    try:
        result = -a * math.exp(-b * math.sqrt(1 / variables_count * sum_squre)) - math.exp(
            1 / variables_count * sum_cx) + a + math.exp(1)
    except Exception as e:
        print(str(e))
    return result
