extends Camera2D

# TODO
# Zoom in moves towards cursor location
# Boundaries
# Smooth slowdown for mouse movement at borders
# Click and drag

const ZOOM_FACTOR = 0.06

var zoomCounter = 0
var zoomType = 0 # 0 for no zoom, 1 for zoom out, -1 for zoom in

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

func _physics_process(delta):

#	if Input.is_key_pressed(KEY_X):
#		print("x: " + String(get_camera_screen_center().x))
#		print("y: " + String(get_camera_screen_center().y))
#		print("x: " + String(get_camera_position().x))
#		print("y: " + String(get_camera_position().y))
#		print("x: " + String(viewport_x))
#		print("y: " + String(viewport_y))
	_calculate_zoom()
	_get_keyboard_input()
	_calculate_border_mouse()

func _input(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == BUTTON_WHEEL_UP: # zoom in
				if get_zoom().x > 0.5 && get_zoom().y > 0.5:
					zoomType = -1
					zoomCounter = 5
					# set_zoom(Vector2(get_zoom().x - 0.1, get_zoom().y - 0.1))
			if event.button_index == BUTTON_WHEEL_DOWN: # zoom out
				if get_zoom().x < 1.3 && get_zoom().y < 1.3:
					zoomType = 1
					zoomCounter = 5
					# set_zoom(Vector2(get_zoom().x + 0.1, get_zoom().y + 0.1))
					
					
func _get_keyboard_input():
	if Input.is_key_pressed(KEY_D):
		translate(Vector2(20, 0))
	if Input.is_key_pressed(KEY_W):
		translate(Vector2(0, -20))
	if Input.is_key_pressed(KEY_A):
		translate(Vector2(-20, 0))
	if Input.is_key_pressed(KEY_S):
		translate(Vector2(0, 20))	

func _calculate_border_mouse():
	
	var viewport_x = get_viewport().get_size().x
	var viewport_y = get_viewport().get_size().y
	
	var mouse_x = get_viewport().get_mouse_position().x
	var mouse_y = get_viewport().get_mouse_position().y
	
	if mouse_x > (viewport_x - 5):
		translate(Vector2(20, 0))
	if mouse_x < (viewport_x - (viewport_x - 5)):
		translate(Vector2(-20, 0))
	if mouse_y > (viewport_y - 5):
		translate(Vector2(0, 20))
	if mouse_y < (viewport_y - (viewport_y - 5)):
		translate(Vector2(0, -20))

func _calculate_zoom():
	if zoomCounter > 0: # smooth zooming
		if zoomType == -1:
			set_zoom(Vector2(get_zoom().x - ZOOM_FACTOR, get_zoom().y - ZOOM_FACTOR))
#			translate(Vector2((get_viewport().get_mouse_position().x - get_camera_position().x),
#					  (get_viewport().get_mouse_position().y - get_camera_position().y)))
		if zoomType == 1:
			set_zoom(Vector2(get_zoom().x + ZOOM_FACTOR, get_zoom().y + ZOOM_FACTOR))
		zoomCounter = zoomCounter - 1	

