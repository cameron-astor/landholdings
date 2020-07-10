extends Node

## NOTES
# This file is getting bloated with different features, will need
# to split it up soon.
#
# Getting data from the world onto the map efficiently is a problem...
#
# Use enums instead of strings for the tile data (e.g. pasture, waste, etc.)
#

# WORLD DIMENSIONS
export var dimensions := Vector2(100, 100)

# PERLIN NOISE PARAMETERS
export var octaves := 4
export var period := 20
export var lacunarity := 1.5
export var persistence := 0.75

# DATA CONTAINERS
var world := []

# CLASSES
var WorldTile

# NODE REFERENCES
onready var map_node: Node2D = get_node("../Map")

# Called when the node enters the scene tree for the first time.
func _ready():
	WorldTile = load("res://Scripts/WorldTile.gd")
	_generate_world()

func _process(delta):
	pass

func _generate_world():
	_generate_terrain()
	_generate_holdings()
	_generate_population()		

func _generate_holdings():
	var holding_id = 0
	var t
	for x in range (dimensions.x):
		for y in range (dimensions.y):
			t = world[x][y]
			if t.agriculture_type == "arable":
				t.holding_id = holding_id
				holding_id += 1

# Uses Open Simplex Noise to generate terrain
func _generate_terrain():
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
			
			var noise_tile = _get_noise_tile(noise.get_noise_2d(float(x), float(y)))
			if noise_tile == AGRICULTURE_TYPES.arable:
				tile.agriculture_type = "arable"
			if noise_tile == AGRICULTURE_TYPES.pasture:
				tile.agriculture_type = "pasture"
			if noise_tile == AGRICULTURE_TYPES.waste:
				tile.agriculture_type = "waste"
			
			world[x].append(tile)
			add_child(tile)			
			
enum AGRICULTURE_TYPES {
	arable,
	pasture,
	waste
}
			
# Takes in a sample of simplex noise and assigns a tile 
# based on the value
func _get_noise_tile(noise_sample):
	if noise_sample < 0.0:
		return AGRICULTURE_TYPES.arable
	if noise_sample < 0.3:
		return AGRICULTURE_TYPES.pasture
	return AGRICULTURE_TYPES.waste
	
func _generate_population():
	var current
	var pop
	for x in range (dimensions.x):
		for y in range (dimensions.y):
			current = world[x][y]
			if current.agriculture_type == "arable":
				pop = randi() % 10
				if (randi() % 3 == 0):
					current.peasant_pop = pop
			
