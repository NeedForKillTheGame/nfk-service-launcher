<?php

// upload text from $_POST['data'] to logs\files\

$max_upload_size = 5024000;

// save to file
if ( isset($_POST['data']) )
{
	if ( strlen($_POST['data']) > $max_upload_size )
		die('Error: data is too long (max size is ' . $max_upload_size . ')');

	// create directory if not exists
	$dir = 'logs/files/';
	if ( !file_exists($dir) )
		mkdir($dir);
	
	$md5 = md5($_POST['data']);
	$filename = $dir . $md5;

	if ( !@file_put_contents($filename, $_POST['data']) )
		die('Error: Can\'t save file to ' . $filename);


	echo $md5;
}

// read from file
if ( isset($_GET['hash']) )
{
	$filename = "logs/files/" . $_GET['hash'];

	if ( $data = file_get_contents($filename) )
		echo $data;
}