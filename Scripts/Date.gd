extends Node

# reference to date object in Data.gd
var date

var month := 1
var year := 0

func _update_date():
	if month == 12:
		year += 1
		month = 1
	else:
		month += 1
