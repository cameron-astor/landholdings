extends Node

class_name WorldTile

# TODO: Use enums instead of strings

# terrain and ecology
var terrain_type: String = "default"
var agriculture_type: String = "default"
var food

# population (test)
var pop: int = 0
var peasant_pop: int = 0
var vagabond_pop: int = 0
var aristocrat_pop: int = 0

# property
var holding_type: String = "default"
var holding_id: int = -1

# Has population been changed in the last tick?
var pop_update_flag: bool = true

# test food
func _update_ecology(month):
	var rand
	if agriculture_type == "arable":
		if month >= 11 || month <= 2: # winter
			food = 0
		if month == 3: # march
			rand = randi() % 3
			if rand == 1:
				food = 1
		if month == 4: # april
			rand = randi() % 3
			if rand == 0:
				food = 1
			if rand == 1 || rand == 2:
				food = 2
		if month == 5: # may
			rand = randi() % 4
			match rand:
				0: food = 2
				1: food = 2
				2: food = 3
				3: food = 5
		if month == 6: # june
			rand = randi() % 3
			match rand:
				0: food = 3
				1: food = 4
				2: food = 5
		if month == 7: # july
			rand = randi() % 3
			match rand:
				0: food = 4
				1: food = 4
				2: food = 5
		if month == 8: # august
			rand = randi() % 4
			match rand:
				0: food = 2
				1: food = 3
				2: food = 3
				3: food = 4
		if month == 9: # september
			rand = randi() % 4
			match rand:
				0: food = 1
				1: food = 1
				2: food = 2
				3: food = 2
		if month == 10: # october
			rand = randi() % 4
			match rand:
				0: food = 0
				1: food = 1
				2: food = 1
				3: food = 1
	else:
		food = 0
					
# Called when the node enters the scene tree for the first time.
#func _ready():
#	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	if pop_update_flag:
#		pop = peasant_pop + aristocrat_pop + vagabond_pop
#		pop_update_flag = false
##	if (randi() % 5 == 0):
##		peasant_pop += 100 # experiment population growth
##		pop_update_flag = true
