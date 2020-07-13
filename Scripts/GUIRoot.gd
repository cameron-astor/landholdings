extends Control

var game_data

# GUI NODES
var date
var FPS

# Called when the node enters the scene tree for the first time.
func _ready():
	game_data = get_node("../../World/Data")
	date = get_node("DateTracker")
	FPS = get_node("FPSCounter")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	_update_date_tracker()
	_update_fps()
	
	
func _update_date_tracker():
	date.set_text(String(game_data.date.month) + ", Year " + String(game_data.date.year))

func _update_fps():
	FPS.set_text("FPS: " + String(Engine.get_frames_per_second()))
