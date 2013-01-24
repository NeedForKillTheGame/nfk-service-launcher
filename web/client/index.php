<?php
require_once('config.php');
require_once('auth.inc.php');


?><!DOCTYPE html>
<html>
    <head>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8">

<link type='text/css' href='css/main.css' rel='stylesheet' media='screen' />
<link type='text/css' href='css/basic.css' rel='stylesheet' media='screen' />

<!-- IE6 "fix" for the close png image -->
<!--[if lt IE 7]>
<link type='text/css' href='css/basic_ie.css' rel='stylesheet' media='screen' />
<![endif]-->

</head>
<body>

<div style='width: 100%'>
	<div style='float: left; margin-left: 10px; color: crimson'>
		<h1>NFK Server Controller</h1>
	</div>
	
	<div style='float: right; text-align: right'>
		Hello, <?php echo $_SESSION['login'] ?> (<a href="?logout=true">log out</a>)
		<br><br>
		<a href="logs/access.log">Log of actions</a>
	</div>
</div>
<div class='clear'></div>



<?php for ($i = 1; $i <= count(Config::$Instances); $i++): ?>

<div class="servers-container">
	<h2><?php echo Config::$Instances[$i]['title'] ?></h2>
	<table class="ko-grid" id="instance<?php echo $i; ?>">
		<thead>
			<tr>
				<th width='75%'>Host Name</th>
				<th width='25%'>Control</th>
			</tr>
		</thead>
		<tbody>
		
		</tbody>
	</table>
	<div id="loading<?php echo $i; ?>" >
		<img src="img/ajax-loader.gif" height="19" width="220" alt="Loading...">
	</div>
</div>

<?php endfor; ?>



<div class='clear'></div>

<div class='description'>
<p>
Временные серваки могут использоваться для проведения турниров или резервироваться для любых игр.
<br>Можно переименовывать их названия соответственно именам команд или игроков, которые должны там играть.
<br>Раз в сутки (утром) они автоматически останавливаются, если все ещё запущены - чтобы не работали "в холостую" и не засоряли планету.
</p>
<p>
Основные серваки автоматически перезапускаются раз в сутки (утром) &mdash; для установки возможных обновлений и достижения максимальной стабильности на весь день.
<br>Но иногда они могут падать (обычно, после дисконнекта с планетой), и в такие трудные моменты им следует помочь с запуском.
</p>
<p>
<i>На планете сортировка делается по аптайму, поэтому при перезапуске сервер всегда оказывается внизу списка.</i>
</p>

</div>
<br />
<div class='author'>&copy; 2013 <a href="http://harpywar.com">HarpyWar</a></div>



<!-- console modal message -->
<div id="basic-modal-content">
<form onsubmit="return false;">
	<h3>NFK Console &mdash; <span id="console-title"></span></h3>
	<p>
	<textarea id="clog" readonly>Initializing console...
</textarea>
	<br>
	<input id="ctext" type="text" value="" />
	</p>
</form>
</div>

<script language="javascript">

var instance_count = <?php echo count(Config::$Instances) ?>;
	
</script>



<script type='text/javascript' src='js/jquery-1.8.3.min.js'></script>
<script type="text/javascript" src="js/noty/jquery.noty.js"></script>
<script type="text/javascript" src="js/noty/layouts/topRight.js"></script>
<script type="text/javascript" src="js/noty/themes/default.js"></script>
<script type='text/javascript' src='js/jquery.simplemodal.js'></script>

<script type='text/javascript' src='js/app.js'></script>
</body>
</html>
