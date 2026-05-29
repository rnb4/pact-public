@tool
extends EditorScript

const DIRECTIONS := ["e", "se", "s", "sw", "w", "nw", "n", "ne"]
const FRAME_SIZE := Vector2(128, 128)
const FPS := 24
const SPRITE_DIR := "res://assets/textures/characters/Longbow/"
const OUTPUT_PATH := "res://assets/sprite_frames/_frames.tres"
const loops := ["walk", "idle", "idle2", "crouch_idle", "run", "strafe_right", "strafe_left", "crouch_run", "slide", "shield_block_mid"]

func _run() -> void:
	var sprite_frames := _build_sprite_frames()
	
	var err = ResourceSaver.save(sprite_frames, OUTPUT_PATH)
	if err == OK:
		print("Saved %d animations to %s" % [sprite_frames.get_animation_names().size(), OUTPUT_PATH])
	else:
		push_error("Failed to save: error %d" % err)


func _build_sprite_frames() -> SpriteFrames:
	var sprite_frames := SpriteFrames.new()
	sprite_frames.remove_animation("default")
	
	var dir := DirAccess.open(SPRITE_DIR)
	if dir == null:
		push_error("Could not open sprite directory: %s" % SPRITE_DIR)
		return
	
	dir.list_dir_begin()
	var file_name = dir.get_next()
	
	while file_name != "":
		if not dir.current_is_dir() and file_name.ends_with(".png"):
			var action: String = file_name.get_basename().to_snake_case()
			var texture_path: String = SPRITE_DIR + file_name
			var texture: Texture2D = load(texture_path)
			
			if texture == null:
				push_warning("Could not load: %s" % texture_path)
				continue
			
			var frame_count := int(texture.get_width() / FRAME_SIZE.x)
			
			for row in range(8):
				var anim_name = "%s_%s" % [action, DIRECTIONS[row]]
				sprite_frames.add_animation(anim_name)
				sprite_frames.set_animation_speed(anim_name, FPS)
				if loops.has(action):
					sprite_frames.set_animation_loop(anim_name, true)
				else:
					sprite_frames.set_animation_loop(anim_name, false)
				
				for col in range(frame_count):
					var atlas := AtlasTexture.new()
					atlas.atlas = texture
					atlas.region = Rect2(
						col * FRAME_SIZE.x,
						row * FRAME_SIZE.y,
						FRAME_SIZE.x,
						FRAME_SIZE.y
					)
					sprite_frames.add_frame(anim_name, atlas)
			
			print("✓ %s -> %s — %d frames per direction" % [file_name, action, frame_count])
		
		file_name = dir.get_next()
	
	dir.list_dir_end()
	
	return sprite_frames
