from .scripts import *

def normalize(input_file):
    print("Processing: delete_with_no_author")
    delete_with_no_author(input_file)

    print("Processing: delete_not_developer")
    delete_not_developer(input_file)

    print("Processing: delete_no_residential_complex")
    delete_no_residential_complex(input_file)

    print("Processing: if_no_address_set_res_compl_as_address")
    if_no_address_set_res_compl_as_address(input_file)

    print("Processing: delete_wrong_price")
    delete_wrong_price(input_file)

    print("Processing: delete_if_no_year_of_construction")
    delete_if_no_year_of_construction(input_file)