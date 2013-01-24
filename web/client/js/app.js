// selected console
var console_handle;
var console_id;
var console_instance_id;
var console_pos = 0;

// array of servers [instance_id][server_id] = server_title
var servers = [];


var i;
$(document).ready(function()
{
	for (i = 1; i <= instance_count; i++)
		loadServers(i);


	// handle enter key to send console command
	$(document).keypress(function(e) {
		if(e.which == 13) {
			scc();
		}
	});
	
	
});


function loadServers(instance_id)
{
	$.getJSON('proxy.php?do=serverlist&instance=' + instance_id, function(data) {
		if (data == null)
		{
			$("#loading" + instance_id).html("Connection error");
			return;
		}

		if (data.result)
		{
			servers[instance_id] = [];
			
			$.each(data.result, function()
			{
				servers[instance_id][this.id] = [];
				servers[instance_id][this.id] = this.hostname;
				
				var html_id = this.id + '_' + instance_id;
				$('#instance' + instance_id + ' > tbody:last').before('<tr><td><a href="#" onclick="return showconsole(' + this.id + ', ' + instance_id + ')" title="Open console"><img id="image' + html_id + '" src="" border=0 width=16 height=16></a>&nbsp;<span class="hostname" id="hostname' + html_id + '" onclick="editname(' + this.id + ', ' + instance_id + ')">' + this.hostname + '</span><input id="hostedit' + html_id + '" onfocusout="savename(' + this.id + ', ' + instance_id + ')" value="' + this.hostname + '"></td><td><button id="button' + html_id + '" onclick="control(' + this.id + ', ' + instance_id + ')"></button></td></tr>');
				changeStatus(html_id, this.status);
			});
			$("#loading" + instance_id).hide();
		}
		if (data.error)
			$("#loading" + instance_id).html(data.error);
	});
}


function control(id, instance_id)
{
	var html_id = id + '_' + instance_id;

	$("#button" + html_id).attr('disabled', true);

	if ( $("#button" + html_id).html() == 'Stop' )
		stop(id, instance_id);
	else
		start(id, instance_id);
}

function start(id, instance_id)
{
	var html_id = id + '_' + instance_id;

	$.getJSON('proxy.php?do=start&id=' + id + '&instance=' + instance_id, function(data) {
		if (data.result)
		{
			msgSuccess('Server ' + $("#hostname" + html_id).html() + ' started');
			changeStatus(html_id, true);
		}
		
		if (data.error)
			msgError(data.error);
	})
	.complete(function() {
		$("#button" + html_id).attr('disabled', false);
	});
}

function stop(id, instance_id)
{
	var html_id = id + '_' + instance_id;
	
	$.getJSON('proxy.php?do=stop&id=' + id + '&instance=' + instance_id, function(data) {
		if (data.result)
		{
			msgSuccess('Server ' + $("#hostname" + html_id).html() + ' stopped');
			changeStatus(html_id, false);
		}
		
		if (data.error)
			msgError(data.error);
	})
	.complete(function() {
		$("#button" + html_id).attr('disabled', false);
	});;
}

function changeStatus(id, status)
{
	if (status)
	{
		$("#hostname" + id).css('color', 'black');
		$("#hostname" + id).css('font-weight', 'bold');
		$("#button" + id).html('Stop');
		$("#image" + id).attr('src', 'img/console_on.png');
	}
	else
	{
		$("#hostname" + id).css('color', 'silver');
		$("#hostname" + id).css('font-weight', 'normal');
		$("#button" + id).html('Start');
		$("#image" + id).attr('src', 'img/console_off.png');
	}

	$("#hostedit" + id).hide();
	$("#hostname" + id).show(3000);
}

function editname(id, instance_id)
{
	var html_id = id + '_' + instance_id;

	$("#hostedit" + html_id).val( unescapeHtml( $("#hostname" + html_id).html() ) );
	
	$("#hostname" + html_id).hide();
	$("#hostedit" + html_id).show().focus();
}

function savename(id, instance_id)
{
	var html_id = id + '_' + instance_id;
	
	// if no changes then exit
	if ( $("#hostname" + html_id).html() == $("#hostedit" + html_id).val() )
	{
		$("#hostedit" + html_id).hide();
		$("#hostname" + html_id).show();
		return;
	}
	/*
	if ( $("#hostname" + html_id).html().indexOf('cleanvoice') !== -1 )
	{
		msgError('You haven\'t permission to edit <b>cleanvoice</b> servers');
		$("#hostedit" + html_id).hide();
		$("#hostname" + html_id).show();
		return;
	}
	*/
	
	$.getJSON('proxy.php?do=editname&id=' + id + '&name=' + escape( $("#hostedit" + html_id).val() ) + '&instance=' + instance_id, function(data) {
	
		if (data.result)
		{
			$("#hostname" + html_id).html( data.result );
			msgAlert('Host name changed. Server restart needed');
		}
		
		if (data.error)
			msgError(data.error);
	})
	.complete(function() {
		$("#hostedit" + html_id).hide();
		$("#hostname" + html_id).show();
	});
}



/* CONSOLE */

function showconsole(id, instance_id)
{
	console_id = id;
	console_instance_id = instance_id;

	// start refresh log each 3 seconds
	console_handle = window.setInterval(function()
	{
		var log = $("#clog");
		$.getJSON('proxy.php?do=getlog&id=' + console_id + '&instance=' + console_instance_id + '&pos=' + console_pos, function(data)
		{
			if (data.result)
			{
				if (data.result.data != '')
				{
					log.html(log.html() + data.result.data );
					log.animate({ scrollTop: document.getElementById('clog').scrollHeight  }, 200);
				}
				console_pos = data.result.pos;
			}
			
			if (data.error)
			{
				msgError(data.error);
				log.html(data.error + '\n\nConsole is deactivated');
				breakconsole();
			}
				
		
		});
	}, 4000);
	
	

	$("#console-title").html(servers[instance_id][id]);
	
	$('#basic-modal-content').modal({ 
		escClose: true, 
		onOpen: function (dialog) {
			
			dialog.overlay.fadeIn('medium', function () {
				dialog.container.fadeIn('fast', function () {
					dialog.data.fadeIn(0, function() {
						$("#ctext").focus();
					});
				});
			});
		},
		onClose: function (dialog) {
			breakconsole();
			$.modal.close(); // must call this!
		}
	});
	
	
	return false;
}

// free console resources
function breakconsole()
{
	window.clearInterval(console_handle); // unset timer
	console_pos = 0;
}

// send console command
function scc()
{
	$.getJSON('proxy.php?do=scc&id=' + console_id + '&instance=' + console_instance_id + '&cmd=' + escape( $("#ctext").val() ), function(data)
	{
		if (data.result)
		{
			$("#ctext").val(''); // clear input
			msgAlert('The command was sent');
		}
		if (data.error)
			msgError(data.error);
			
	});
	
}

function msgSuccess(text)
{
	noty({text: text, timeout: 2000, type: 'success', layout: 'topRight' });
}
function msgAlert(text)
{
	noty({text: text, timeout: 3000, type: 'alert', layout: 'topRight' });
}
function msgError(text)
{
	noty({text: text, timeout: 5000, type: 'error', layout: 'topRight' });
}

function unescapeHtml(unsafe) {
  return unsafe
      .replace(/&amp;/g, "&")
      .replace(/&lt;/g, "<")
      .replace(/&gt;/g, ">")
      .replace(/&quot;/g, '"')
      .replace(/&#039;/g, "'");
}
