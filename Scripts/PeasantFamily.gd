extends Node

var id
var size
var holdings := []
var food_supply

## Called when the node enters the scene tree for the first time.
#func _ready():
#	pass # Replace with function body.
#
## Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

func _sow():
	pass

func _harvest():
	pass

func _metabolize():
	food_supply -= size
