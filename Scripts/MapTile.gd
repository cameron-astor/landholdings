extends Sprite

class_name MapTile

var x
var y
var current_map_mode
var assigned_tile

# terrain colors
const arable_color := Color("5F6B41")
const pasture_color := Color("DBC164")
const waste_color := Color("723E1B")

enum MAPMODES {
	Terrain,
	Peasant,
	Landholdings
}

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	# Terrain
	if current_map_mode == MAPMODES.Terrain:
		if assigned_tile.agriculture_type == "arable":
			self.modulate = arable_color
		if assigned_tile.agriculture_type == "pasture":
			self.modulate = pasture_color	
		if assigned_tile.agriculture_type == "waste":
			self.modulate = waste_color
