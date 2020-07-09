extends Node

class_name WorldTile

# TODO: Use enums instead of strings

var terrain_type: String = "default"
var agriculture_type: String = "default"
var pop: int = 0
var peasant_pop: int = 0
var vagabond_pop: int = 0
var aristocrat_pop: int = 0
var holding_type: String = "default"
var holding_id: int = -1

# Has population been changed in the last tick?
var pop_update_flag: bool = true


# Called when the node enters the scene tree for the first time.
#func _ready():

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if pop_update_flag:
		pop = peasant_pop + aristocrat_pop + vagabond_pop
		pop_update_flag = false
	if (randi() % 5 == 0):
		peasant_pop += 100 # experiment population growth
		pop_update_flag = true
