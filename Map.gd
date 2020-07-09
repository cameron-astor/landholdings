extends Node2D

########################################
## 				MAP                   ##
## Displays the underlying game data. ##
########################################

## TODO:
# Take user input code out of here and put in another node
#
#

# reference to world data
onready var world_data: Node = get_node("../Data") 
var world

# 2D array of sprites
var map := []
var width
var height

# base map tile
var base_tile

# terrain colors
const arable_color := Color("5F6B41")
const pasture_color := Color("DBC164")
const waste_color := Color("723E1B")

# map modes
var current_map_mode = 0
var map_modes = ["terrain", "peasant", "landholdings"]
	
################	
## USER INPUT ##
################
var pressed: bool = false # jank as fuck, use the better way soon
func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT && event.pressed:
			_print_tile_summary()
			
	# switch mapmode
	if event.is_action("mapmode_forward") and not event.echo and not pressed:
			if current_map_mode == map_modes.size() - 1:
				current_map_mode = 0
			else:
				current_map_mode += 1
			print("switch map mode to " + map_modes[current_map_mode])
			pressed = true
			
	if event.is_action("mapmode_forward") and event.pressed == false:
			pressed = false
			
# Called when the node enters the scene tree for the first time.
func _ready():
	base_tile = load("res://Graphics/Tile.png")
	
	width = world_data.dimensions.x
	height = world_data.dimensions.y
	
	# initialize map to empty
	for x in range (width):
		map.append([])
		for y in range(height):
			map[x].append(0)
			
	world = world_data.world
	_generate_map()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	_update_map()
	# print(map[50][50].modulate)

# First time map gen
func _generate_map():
	
	for x in range (width):
		for y in range (height):
			var sprite = Sprite.new()
			sprite.texture = base_tile
			sprite.position.x = x * 16
			sprite.position.y = y * 16	
			
			map[x][y] = sprite
			add_child(sprite)

func _print_tile_summary():
	var tile_summary := {}
	# DEBUG
	var coordinates = Vector2(
		round(get_global_mouse_position().x/16),
		round(get_global_mouse_position().y/16)
		)
	if coordinates.x < width and coordinates.y < height:
		var tile = world[coordinates.x][coordinates.y]
		tile_summary["1. Coordinates"] = coordinates
		tile_summary["2. Agriculture"] = tile.agriculture_type
		tile_summary["3. Population Total"] = tile.pop
	#	tile_summary["4. Peasants"] = tile_dict[coordinates].peasant_pop
	#	tile_summary["5. Aristocrats"] = tile_dict[coordinates].aristocrat_pop
	#	tile_summary["6. Vagabonds"] = tile_dict[coordinates].vagabond_pop
		tile_summary["7. Holding ID"] = tile.holding_id
		print(tile_summary)	
	
###########################
## MAP DISPLAY FUNCTIONS ##
###########################

# Updates the map display.
# TODO: How can we only update when we need too? (far too inefficient currently)
func _update_map():
	if map_modes[current_map_mode] == "peasant":
		var current
		var alpha
		for x in range (width):
			for y in range (height):
				current = world[x][y]
				alpha = float(current.peasant_pop) / (current.peasant_pop + 100000)
				map[x][y].modulate = Color(0,0,1, alpha) 
	if map_modes[current_map_mode] == "terrain":
		var current
		for x in range (width):
			for y in range (height):
				current = world[x][y]
				if current.agriculture_type == "arable":
					map[x][y].modulate = arable_color
				if current.agriculture_type == "pasture":
					map[x][y].modulate = pasture_color	
				if current.agriculture_type == "waste":
					map[x][y].modulate = waste_color	
