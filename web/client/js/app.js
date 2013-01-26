// selected console
var console_handle = 0;
var console_id = 0;
var console_instance_id;
var console_pos = 0;

// array of servers [instance_id][server_id] = server_title
var servers = [];

var i;
$(document).ready(function()
{
	for (i = 1; i <= instance_count; i++)
		loadServers(i);

		
	// handle enter key to lose focuse from input (when change server name)
	$(".servers-container :input").live('keypress', function(e) {
		if(e.which == 13) {
			$(this).blur();
			return false;
		}
	});
		
	// handle enter key to send console command
	$("#ctext").keypress(function(e) {
		if(e.which == 13) {
			scc();
			return false;
		}
	});
	
	$( "#tabs" ).tabs({
	  activate: function( event, ui ) {
		loadfile( console_id, console_instance_id, getTabSelectedIndex() );
	  }
	});
	
	// set tab orientation = vertical
	$( "#tabs" ).tabs().addClass( "ui-tabs-vertical ui-helper-clearfix" );
    $( "#tabs li" ).removeClass( "ui-corner-top" ).addClass( "ui-corner-left" );
	
	// save and cancel buttons in files dialog
	$("#fsave").click(function(e) {
		savefile( console_id, console_instance_id, getTabSelectedIndex() );
	});
	$("#fcancel").click(function(e) {
		$("#file-modal").dialog('close');
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
				servers[instance_id][this.id].hostname = this.hostname;
				servers[instance_id][this.id].status = this.status;
				
				var html_id = this.id + '_' + instance_id;
				$('#instance' + instance_id + ' > tbody:last').before('<tr><td><a href="#" onclick="return showfiles(' + this.id + ', ' + instance_id + ')" title="Config editor"><img id="config_image' + html_id + '" src="img/config.png" border=0 width=16 height=16></a>&nbsp;<a href="#" onclick="return showconsole(' + this.id + ', ' + instance_id + ')" title="Console"><img id="console_image' + html_id + '" src="" border=0 width=16 height=16></a>&nbsp;<span class="hostname" id="hostname' + html_id + '" onclick="editname(' + this.id + ', ' + instance_id + ')">' + this.hostname + '</span><input id="hostedit' + html_id + '" onfocusout="savename(' + this.id + ', ' + instance_id + ')" value="' + this.hostname + '"></td><td><button id="button' + html_id + '" onclick="control(' + this.id + ', ' + instance_id + ')"></button></td></tr>');
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
			msgSuccess('Server ' + $("#hostname" + html_id).html() + ' stopped');
			changeStatus( html_id, true );
		}
		else
		{
			if (data.error)
				msgError(data.error);
				
			getServerStatus(id, instance_id)
		}
		//console.log(data.result);
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
			changeStatus( html_id, false );
		}
		else
		{
			if (data.error)
				msgError(data.error);
				
			getServerStatus(id, instance_id)
		}
		//console.log(data.result);
	})
	.complete(function() {
		$("#button" + html_id).attr('disabled', false);
	});
}

// get server status from api
function getServerStatus(id, instance_id)
{
	var html_id = id + '_' + instance_id;
	var status = false;
	
	$.getJSON('proxy.php?do=status&id=' + id + '&instance=' + instance_id, function(data) {
		if (data.result)
			status = data.result;
		
		changeStatus( html_id, status );
	});
}


function changeStatus(id, status)
{
	if (status)
	{
		$("#hostname" + id).css('color', 'black');
		$("#hostname" + id).css('font-weight', 'bold');
		$("#button" + id).html('Stop');
		$("#console_image" + id).attr('src', 'img/console_on.png');
	}
	else
	{
		$("#hostname" + id).css('color', 'silver');
		$("#hostname" + id).css('font-weight', 'normal');
		$("#button" + id).html('Start');
		$("#console_image" + id).attr('src', 'img/console_off.png');
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

	var log = $("#clog");
	log.html('Initializing console...\n');
	
	// start refresh log each 3 seconds
	console_handle = window.setInterval(function()
	{
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
	
	// set console visual style depending server status
	if ( servers[instance_id][id].status )
	{
		log.removeClass('console-off').addClass('console-on');
		$("#ctext").attr('disabled', false);
	}
	else
	{
		log.removeClass('console-on').addClass('console-off');
		$("#ctext").attr('disabled', true);
	}
	
	// console modal window
	$("#console-modal").attr('title', "NFK Console — " + servers[instance_id][id].hostname);
	$('#console-modal').dialog({
		width: 800,
		height: 575,
		modal: true,
		show: {
			effect: "explode",
			duration: 200
		},
		hide: {
			effect: "explode",
			duration: 500
		},
		close: function(event, ui) {
			breakconsole();
			$(this).dialog('destroy');
		}
    });

	
	// set focus on console input
	$(document).bind('keydown', function() {
		$("input#ctext").focus();
	});
	
	
	return false;
}

// free console resources
function breakconsole()
{
	window.clearInterval(console_handle); // unset timer
	console_handle = 0;
	console_pos = 0;
	
	// unbind console focus on keydown
	$(document).unbind('keydown');
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



/* CONFIG EDITOR */

function showfiles(id, instance_id)
{
	console_id = id;
	console_instance_id = instance_id;

	
	// first tab is activated so update content there
	loadfile( console_id, console_instance_id, getTabSelectedIndex() );

	// config modal window
	$("#file-modal").attr('title', 'NFK Config — ' + servers[instance_id][id].hostname);
	$('#file-modal').dialog({
		width: 800,
		height: 575,
		modal: true,
		show: {
			effect: "explode",
			duration: 200
		},
		hide: {
			effect: "explode",
			duration: 500
		},
		close: function(event, ui) {
			$(this).dialog('destroy');
		},
    });

	return false;
}

// load remote file into textarea
function loadfile(id, instance_id, tab_index)
{
	var filename = $("#tabs>ul>li>a[href=#tabs-" + tab_index + "]").text()
	var textarea = $("#tabs>div#tabs-" + tab_index + ">textarea");

	// disable button
	$("#fsave").attr('disabled', true);
	textarea.attr('disabled', true); // disable edit text

	$.getJSON('proxy.php?do=getfile&id=' + console_id + '&instance=' + console_instance_id + '&file=' + filename, function(data)
	{
		if (data.result != '')
		{
			textarea.val(data.result);
			
			// enable button
			$("#fsave").attr('disabled', false);
			textarea.attr('disabled', false); // enable edit text
			textarea.focus();
		}
		
		if (data.error)
		{
			msgError(data.error);
			textarea.val(data.error);
			$("#fsave").attr('disabled', true);
			textarea.attr('disabled', true);
		}
	}).complete(  ); 
	
}

// send file from textarea to server
function savefile(id, instance_id, tab_index)
{
	var filename = $("#tabs>ul>li>a[href=#tabs-" + tab_index + "]").text()
	var textarea = $("#tabs>div#tabs-" + tab_index + ">textarea");
	
	$("#fsave").attr('disabled', true);
	
	$.post('file.php', { data: textarea.val() }, function(data)
	{
		//console.log(data);
		
		// md5 hash
		if (data.length == 32)
		{
			var url = window.location.origin + window.location.pathname + 'file.php?hash=' + data;
			//console.log(url);
			
			msgAlert('Sending ' + filename + ' ...');
			
			// send api request to update file on the server
			$.getJSON('proxy.php?do=savefile&id=' + console_id + '&instance=' + console_instance_id + '&file=' + filename + '&url=' + escape(url), function(data)
			{
				msgSuccess(filename + " updated on the server");
				
				loadfile(id, instance_id, tab_index);
			});
		}
		else
			msgError(data);

	});
}


// return files tab selected index
function getTabSelectedIndex()
{
	return $("#tabs").tabs('option').active;
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
