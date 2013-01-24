<?php

session_start();

// try to find user to auth
if ( isset($_SERVER['PHP_AUTH_USER']) )
	foreach(Config::$Users as $user => $pass)
		if ( $user == $_SERVER['PHP_AUTH_USER'] && $pass == $_SERVER['PHP_AUTH_PW'] )
		{
			// remember username
			
			$_SESSION['login'] = $_SERVER['PHP_AUTH_USER'];
		}




// basic auth window with login and pass
if (!isset($_SESSION['login']))
	authenticate();

// logout
if ( isset($_GET['logout']) )
{
	unset($_SERVER['PHP_AUTH_USER']);
	unset($_SERVER['PHP_AUTH_PW']);

	session_unset();
	session_destroy();
	
	header('HTTP/1.0 401 Unauthorized');
	
	echo "Session closed. <a href='.'>Login again</a>?";
	exit();
	
	//authenticate();
}



function authenticate()
{
	header('WWW-Authenticate: Basic realm="NFK Realm"');
	header('HTTP/1.0 401 Unauthorized');
	
	if ( $_SERVER["SCRIPT_FILENAME"] == 'proxy.php' )
		throw new Exception('Access denied');
	
	echo "Access denied.";
	exit;
}