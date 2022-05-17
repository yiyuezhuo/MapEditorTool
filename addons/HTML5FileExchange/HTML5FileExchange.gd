extends Node

signal read_completed(data, dataType)
# signal load_completed(image)

var js_callback = JavaScript.create_callback(self, "load_handler");
var js_interface;

func _ready():
	if OS.get_name() == "HTML5" and OS.has_feature('JavaScript'):
		_define_js()
		js_interface = JavaScript.get_interface("_HTML5FileExchange");

func _define_js()->void:
	#Define JS script
	JavaScript.eval("""
	var _HTML5FileExchange = {};
	_HTML5FileExchange.upload = function(gd_callback, accept) {
		canceled = true;
		var input = document.createElement('INPUT'); 
		input.setAttribute('type', 'file');
		if(typeof(accept) !== 'undefined'){
			input.setAttribute('accept', accept);
		}
		// input.setAttribute('accept', 'image/png, image/jpeg, image/webp');
		input.click();
		input.addEventListener('change', event => {
			if (event.target.files.length > 0){
				canceled = false;}
			var file = event.target.files[0];
			var reader = new FileReader();
			this.fileType = file.type;
			// var fileName = file.name;
			reader.readAsArrayBuffer(file);
			reader.onloadend = (evt) => { // Since here's it's arrow function, 'this' still refers to _HTML5FileExchange
				if (evt.target.readyState == FileReader.DONE) {
					this.result = evt.target.result;
					gd_callback(); // It's hard to retrieve value from callback argument, so it's just for notification
				}
			}
		  });
	}
	""", true)

func load_handler(_args):
	var dataType = js_interface.fileType;
	var data = JavaScript.eval("_HTML5FileExchange.result", true) # interface doesn't work as expected for some reason
	emit_signal("read_completed", data, dataType)

func read_data(accept : String):
	js_interface.upload(js_callback, accept)

func save_data(buffer:PoolByteArray, fileName:String)->void:
	JavaScript.download_buffer(buffer, fileName)