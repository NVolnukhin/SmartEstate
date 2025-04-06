import sys
from price_history_plotter import get_price_chart_base64

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python cli_plotter.py <flat_id>")
        sys.exit(1)

    try:
        flat_id = int(sys.argv[1])
        result = get_price_chart_base64(flat_id)
        print(result)
    except ValueError:
        print("Error: flat_id must be an integer")
        sys.exit(1)
    except Exception as e:
        print(f"Error: {str(e)}")
        sys.exit(1)