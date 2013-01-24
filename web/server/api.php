<?php
// NFK Server API Controller
// (c) 2013 HarpyWar (harpywar@gmail.com)
// http://harpywar.com


require_once("config.php");

$response = array();



if ( !isset($_GET['do']) )
	die('No action');

// function to execute
$action = $_GET['do']; 

// allowed functions
if ($action == 'serverlist' ||
	$action == 'status' ||
	$action == 'start' ||
	$action == 'stop' ||
	$action == 'restart' ||
	$action == 'editname' ||
	$action == 'getlog' ||
	$action == 'scc' ||
	$action == 'getfile' ||
	$action == 'editfile')
try
{
	// auth
	if ( !isset($_GET['apikey']) || $_GET['apikey'] != Config::$ApiKey )
		throw new Exception('Authorization failed');

	// server id
	$id = (isset($_GET['id'])) ? $_GET['id'] : false;
	
	// call function by name
	$response['result'] = $action($id);
	
	// convert result to utf-8
	$response['result'] = _encoding($response['result']);
}
catch(Exception $e)
{
	$response['error'] = $e->GetMessage();
}


// send response to client
echo json_encode($response);
exit();





// recursively encode string to utf-8
function _encoding($val)
{
	if ( is_string($val) )
		$val = iconv('cp1251', 'utf-8', $val);
	
	if ( is_array($val) )
		foreach ($val as $k => $v)
				$val[$k] = _encoding($v);
				
	return $val;
}





// TODO: серверы останавливаются, если планета недоступна
// TODO: серверы нельзя запустить, если планета недоступна




/* METHODS */

function serverlist()
{
	$serverlist = array();
	// fill serverlist with actual status
	foreach (Config::$Servers as $sid => $sport)
		$serverlist[$sid + 1] = _getserverstatus($sid + 1);

	// TODO: get status from planet?
	
	return $serverlist;
}

function _getserverstatus($id)
{
	$item = array();
	
	$item['id'] = $id;
	$item['status'] = status($id);
	$item['hostname'] = _gethostname($id);
	
	return $item;
}
// return sv_hostname from config
function _gethostname($id)
{
	$hostname = '';
	$_GET['file'] = 'server.cfg';
	$data = getfile($id);
	
	if ( $find = stristr($data, 'sv_hostname') )
	{
		$find = str_split(substr($find, 11, strlen($find) - 11) );
		$chr = '';
		while ($chr != "\n")
		{
			if ($chr = next($find))
				$hostname .= $chr;
			else
				break;
		}
	}
	return _getcolorname($hostname);
}
function _getcolorname($val)
{
	$val = rtrim($val, "\r\n");
	// TODO: colored text?
	
	return $val;
}




// start service and return action result (true | false)
function start($id)
{
    $result = _cmd($id, 'start');
	$status = strstr($result, 'START_PENDING') ? true : false;;

	return $status;
}
// stop service and return action result (true | false)
function stop($id)
{
    $result = _cmd($id, 'stop');
	$status = ( strstr($result, 'STOP_PENDING') || strstr($result, 'STOPPED') ) ? true : false;;

	return $status;
}
// return status of the service (true | false)
function status($id)
{
    $result = _cmd($id, 'query');
	$status = strstr($result, 'RUNNING') ? true : false;
	
	return $status;
}

function restart($id)
{
	stop($id);
	return start($id);
}

// execute system command and return output result
function _cmd($id, $action)
{
	$port = _getportbyid($id);

	$cmd = "script\\control.cmd $action $port";
    $result = shell_exec($cmd);

	// result example:
	if ( strstr($result, '[SC] ') )
		throw new Exception($result);
	
	return $result;
	
	// STOP
	/*
	SERVICE_NAME: NFK_29995
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 1  STOPPED
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
	*/
	// START
	/*
	SERVICE_NAME: NFK_29995
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 2  START_PENDING
                                (NOT_STOPPABLE, NOT_PAUSABLE, IGNORES_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x7d0
        PID                : 11016
        FLAGS              :
	*/
	// STATUS
	/*
	SERVICE_NAME: NFK_29995
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 4  RUNNING
                                (STOPPABLE, PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
	*/
	// ACCESS DENIED
	/*
	[SC] OpenService: ®иЁЎЄ : 5: ЋвЄ § ­® ў ¤®бвгЇҐ.
	or
	[SC] StartService: OpenService FAILED 5: Access is denied.
	or
	[SC] EnumQueryServicesStatus:OpenService FAILED 5: Access is denied.
	*/
}

function editname($id)
{
	$oldname = _gethostname($id);

	if ( !isset($_GET['name']) )
		throw new Exception('Hostname can\'t be empty!');
	
	$newname = $_GET['name'];
	if ($newname == $oldname)
		return false;

	// replace old to new
	$data = getfile($id);

	$data = str_replace("sv_hostname {$oldname}", "sv_hostname {$newname}", $data);

	// save changes
	$_POST['data'] = $data;
	editfile($id);
	
	return $newname;
}


// print log data from the specified position $_GET['pos']
function getlog($id)
{
	// max content size to output
	$max_read_size = 20000;

	$_GET['file'] = 'realtime.log';	
	$filename = _getfilename($id);
	
	if ( !file_exists($filename) )
		throw new Exception('The server doesn\'t support web console');
	
	
	// position in file from where to start reading
	$pos = isset($_GET['pos']) ? $_GET['pos'] : 0;
	
	$f = fopen($filename, 'r');
	$filesize = filesize($filename);
	
	// if pos not set or value invalid then calc it from end of file
	if ($pos == 0 || $pos > $filesize)
	{
		$pos = ($filesize - $max_read_size > 0) ? $filesize - $max_read_size : 0;
	
		fseek($f, $pos);

		// find a carriege return from current pos
		$prev = '';
		while ( !feof($f) )
		{
			$c = fread($f, 1);
			if ($prev == "\r" && $c == "\n")
			{
				$pos = ftell($f);
				break;
			}

			$prev = $c;
		}
	}
	
	$length = ($filesize - $pos > $max_read_size) ? $max_read_size : $filesize - $pos;

	$data = '';
	if ($length > 0)
	{
		fseek($f, $pos);
		$data = fread($f, $length);
		$pos = ftell($f);
	}

	fclose($f);

	return array('pos' => $pos, 'data' => $data);
}

// send console message
function scc($id)
{
	$_GET['file'] = 'scc.cfg';	
	$filename = _getfilename($id);
	
	if ( !isset($_GET['cmd']) || strlen(trim($_GET['cmd'])) == 0 )
		throw new Exception('Console command can\'t be empty');
	
	$cmd = "\n" . _utf8_urldecode($_GET['cmd']);;
	$cmd = iconv("UTF-8", "CP1251", $cmd);
	
	// append command to the scc.cfg (it will be executed by bot.dll)
	if ( !@file_put_contents($filename, $cmd, FILE_APPEND) )
		throw new Exception('Can\'t write to ' . $filename);

	// return position
	return true;
}
// decode escaped json utf-8 encoded string
function _utf8_urldecode($str)
{
    $str = preg_replace("/%u([0-9a-f]{3,4})/i","&#x\\1;", urldecode($str));
    return html_entity_decode($str,null,'UTF-8');
}

// get file content
function getfile($id)
{
	$filename = _getfilename($id);
	
	if ( !$data = @file_get_contents($filename) )
		throw new Exception('Can\'t read ' . $filename);

	return $data;
}
// put new content from $_POST['data'] to the file
function editfile($id)
{
	$filename = _getfilename($id);
	
	if ( !isset($_POST['data']) )
		throw new Exception('Empty file content');
	
	$data = $_POST['data']; // file content
	
	if ( !@file_put_contents($filename, $data) )
		throw new Exception('Can\'t save ' . $filename);
	
	return true;
}

// get filename full path from predefine names
function _getfilename($id)
{
	if ( !isset($_GET['file']) )
		throw new Exception('Empty filename');

	$file = $_GET['file']; // file name
	
	
	$path = '';
	
	switch($file)
	{
		case 'maplist.txt':
			$path = '\BASENFK\\';
			break;
		case 'realtime.log':
		case 'scc.cfg':
		case 'server.cfg':
		case 'startup.cfg':
			$path = '\SERVER\\';
			break;
			
		default:
			throw new Exception('Bad filename ' . $file);
	}

	$filename = Config::$ServerPath . _getportbyid($id) . $path . $file;

	return $filename;
}
// get port by server id
function _getportbyid($id)
{
	foreach (Config::$Servers as $sid => $sport)
		if ($id == $sid + 1)
			return $sport;
	
	throw new Exception('Bad server ID #' . $id);
}