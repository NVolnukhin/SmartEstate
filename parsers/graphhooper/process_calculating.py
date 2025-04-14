from .calculator import WalkingTimeCalculator


def process_time_calculating():
    calculator = WalkingTimeCalculator()
    calculator.process_buildings(batch_size=27)