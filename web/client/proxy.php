<?php
require_once('config.php');

// proxy gateway to another instances
if ( isset($_GET['instance']) )
{
	$api_url = ( array_key_exists($_GET['instance'], Config::$Instances) )
		? Config::$Instances[$_GET['instance']]['url']
		: false;
	
	try
	{
		require_once('auth.inc.php');
		
		if (!$api_url)
			throw new Exception('Bad instance ID');
	
		
		if ( $data = file_get_contents( $api_url . '&' . $_SERVER['QUERY_STRING'] ) )
		{
			echo $data;
			
			// log all actions except frequent commands
			if ($_GET['do'] != 'serverlist' && $_GET['do'] != 'getlog' && $_GET['do'] != 'getfile')
				log_action( $_SESSION['login'], $_SERVER['QUERY_STRING'] );
				
			exit;
		}
		throw new Exception('API transfer failed');
	}
	catch(Exception $e)
	{
		echo "{ 'error': 'Internal error. " . $e->getMessage() . "' }";
	}
}

function log_action($login, $line)
{
	$line = date("[d.m.Y, H:i]") . "\t[{$login}]\t{$line}\n";
	@file_put_contents('logs/access.log', $line, FILE_APPEND);
}