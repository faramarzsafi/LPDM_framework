import output
from tests.genetic import run_ga
from tests.pso import run_pso

if __name__ == '__main__':
    output.result_output_path = "_results"
    output.excel_path = "Template.xlsx"
    run_ga()
    # run_pso()
