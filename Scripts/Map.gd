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
# World tile data
var world
# Dictionary of holdings to peasant family
var peasant_holdings

# 2D array of sprites
var map := []
var width
var height

# base map tile
var base_tile

# MapTile class
var MapTile

# terrain colors
const arable_color := Color("5F6B41")
const pasture_color := Color("DBC164")
const waste_color := Color("723E1B")

# holdings colormap
var holding_colors := {}
var peasant_colors := {}

# map modes
var current_map_mode = 0
var map_modes = ["terrain", "population", "landholdings", 
				"peasants", "food"]
	
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
	MapTile = load("res://Scripts/MapTile.gd")
	
	width = world_data.dimensions.x
	height = world_data.dimensions.y
	
	# initialize map to empty
	for x in range (width):
		map.append([])
		for y in range(height):
			map[x].append(0)
			
	world = world_data.world
	peasant_holdings = world_data.peasant_holdings
	_generate_map()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	_update_map()

# First time map gen
func _generate_map():
	
	for x in range (width):
		for y in range (height):
			
			# Create tile sprite
			var sprite = Sprite.new()
			sprite.texture = base_tile
			sprite.position.x = x * 16
			sprite.position.y = y * 16	

			# Add holding data to color map
			var color = Color(rand_range(0,1), rand_range(0,1), rand_range(0,1))
			var holding_id = world[x][y].holding_id
			if holding_id != -1:
				holding_colors[holding_id] = color
				
			# Add peasant data to color map
			color = Color(rand_range(0,1), rand_range(0,1), rand_range(0,1))
			holding_id = world[x][y].holding_id
			if holding_id != -1:
				var peasant_id = peasant_holdings[world[x][y].holding_id].id
				peasant_colors[peasant_id] = color
			
### Possible MapTile arrangement
#			var sprite = MapTile.new()
#			sprite.texture = base_tile
#			sprite.position.x = x * 16
#			sprite.position.y = y * 16
#			sprite.assigned_tile = world[x][y]
#			sprite.current_map_mode = sprite.MAPMODES.Terrain
				
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
		tile_summary["4. Holding ID"] = tile.holding_id
		if tile.holding_id != -1:
			tile_summary["5. Peasant Family ID"] = peasant_holdings[tile.holding_id].id
		else:
			tile_summary["5. Peasant Family ID"] = "No peasants"
		tile_summary["6. Food"] = tile.food
		
		print(tile_summary)	
	
###########################
## MAP DISPLAY FUNCTIONS ##
###########################

# Updates the map display.
# TODO: How can we only update when we need too? (far too inefficient currently)
func _update_map():
	
	var current
	var alpha
	for x in range (width):
		for y in range (height):
			
			if map_modes[current_map_mode] == "population":			
				current = world[x][y]
				alpha = float(current.peasant_pop) / (current.peasant_pop + 100)
				map[x][y].modulate = Color(0,0,1, alpha) 
				
			if map_modes[current_map_mode] == "terrain":
				current = world[x][y]
				if current.agriculture_type == "arable":
					map[x][y].modulate = arable_color
				if current.agriculture_type == "pasture":
					map[x][y].modulate = pasture_color	
				if current.agriculture_type == "waste":
					map[x][y].modulate = waste_color	
					
			if map_modes[current_map_mode] == "landholdings":
				current = world[x][y].holding_id
				if current != -1:
					map[x][y].modulate = holding_colors[current]
						
			if map_modes[current_map_mode] == "peasants":
				current = world[x][y].holding_id
				if current != -1:
					map[x][y].modulate = peasant_colors[peasant_holdings[current].id]
					
			if map_modes[current_map_mode] == "food":
				current = world[x][y].food
				alpha = float(current) / (current + 5)
				map[x][y].modulate = Color(0,1,0, alpha) 					
