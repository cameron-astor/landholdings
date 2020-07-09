extends Node

## NOTES
# This file is getting bloated with different features, will need
# to split it up soon.
#
# Getting data from the world onto the map efficiently is a problem...
#
#
#

# terrain colors
const arable_color := Color("5F6B41")
const pasture_color := Color("DBC164")
const waste_color := Color("723E1B")

export var dimensions := Vector2(100, 100)

# Perlin noise parameters
export var octaves := 4
export var period := 20
export var lacunarity := 1.5
export var persistence := 0.75

var world := []
var tile_dict := {}
var sprite_dict := {}
var WorldTile

onready var world_node: Node2D = get_node("../World")

var pressed: bool = false # jank as fuck, use the better way soon

# map modes
var current_map_mode = 0
var map_modes = ["terrain", "peasant", "landholdings"]

# Called when the node enters the scene tree for the first time.
func _ready():
	WorldTile = load("res://Scripts/WorldTile.gd")
	_generate_world_sprite()

func _process(delta):
	_update_map()
	
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

func _generate_world_sprite():
	var base_tile = load("res://Graphics/Tile.png")
	_generate_terrain(base_tile)
	_generate_holdings()		

func _generate_holdings():
	var holding_id = 0
	for t in tile_dict.values():
		if t.agriculture_type == "arable":
			t.holding_id = holding_id
			holding_id += 1
			
func _print_tile_summary():
	var tile_summary := {}
	# DEBUG
	var coordinates = Vector2(
		abs(round(world_node.get_global_mouse_position().x/16)),
		abs(round(world_node.get_global_mouse_position().y/16))
		)
	var tile = tile_dict[coordinates]
	tile_summary["1. Coordinates"] = coordinates
	tile_summary["2. Agriculture"] = tile.agriculture_type
	tile_summary["3. Population Total"] = tile.pop
#	tile_summary["4. Peasants"] = tile_dict[coordinates].peasant_pop
#	tile_summary["5. Aristocrats"] = tile_dict[coordinates].aristocrat_pop
#	tile_summary["6. Vagabonds"] = tile_dict[coordinates].vagabond_pop
	tile_summary["7. Holding ID"] = tile.holding_id
	print(tile_summary)	

# Uses Open Simplex Noise to generate terrain
func _generate_terrain(base_tile: Texture):
	randomize() # reseed rng
	var noise = OpenSimplexNoise.new()
	noise.seed = randi()
	noise.octaves = octaves
	noise.period = period
	noise.lacunarity = lacunarity
	noise.persistence = persistence
	
	for x in range (dimensions.x):
		world.append([])
		for y in range (dimensions.y):
			# Generate tile for the current x,y position
			var tile = WorldTile.new()
			
			var sprite = Sprite.new()
			sprite.texture = base_tile
			sprite.position.x = x * 16
			sprite.position.y = y * 16
			
			var noise_tile = _get_noise_tile(noise.get_noise_2d(float(x), float(y)))
			sprite.modulate = Color(noise_tile)
			if noise_tile == arable_color:
				tile.agriculture_type = "arable"
			if noise_tile == pasture_color:
				tile.agriculture_type = "pasture"
			if noise_tile == waste_color:
				tile.agriculture_type = "waste"
			
			tile_dict[Vector2(x, y)] = tile
			sprite_dict[Vector2(x, y)] = sprite
			world_node.add_child(sprite)
			# SUPER SUBOPTIMAL FIX THIS SHIT
			world_node.add_child(tile)			
			
# Takes in a sample of simplex noise and assigns a tile 
# based on the value
func _get_noise_tile(noise_sample):
	if noise_sample < 0.0:
		return arable_color
	if noise_sample < 0.3:
		return pasture_color
	return waste_color
	
func _generate_population():
	pass

###########################
## MAP DISPLAY FUNCTIONS ##
###########################

# Updates the map display.
# TODO: How can we only update when we need too? (far too inefficient currently)
func _update_map():
	if map_modes[current_map_mode] == "peasant":
		var current
		var alpha
		for x in range (dimensions.x):
			for y in range (dimensions.y):
				current = tile_dict[Vector2(x, y)]
				alpha = float(current.peasant_pop) / (current.peasant_pop + 100000)
				sprite_dict[Vector2(x, y)].modulate = Color(0,0,1, alpha) 
	if map_modes[current_map_mode] == "terrain":
		var current
		for x in range (dimensions.x):
			for y in range (dimensions.y):
				current = tile_dict[Vector2(x, y)]
				if current.agriculture_type == "arable":
					sprite_dict[Vector2(x, y)].modulate = arable_color
				if current.agriculture_type == "pasture":
					sprite_dict[Vector2(x, y)].modulate = pasture_color	
				if current.agriculture_type == "waste":
					sprite_dict[Vector2(x, y)].modulate = waste_color	
