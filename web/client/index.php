<?php
require_once('config.php');
require_once('auth.inc.php');


?><!DOCTYPE html>
<html>
    <head>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
	<link type='text/css' href='css/main.css' rel='stylesheet' media='screen' />

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
<img id="config_image1_2" src="img/config.png" border="0" width="16" height="16"> Редактор конфигурационных файлов сервера
<br>
<img id="console_image1_2" src="img/console_on.png" border="0" width="16" height="16"> Консоль сервера, позволяет отправлять команды
</p>
<p>
Cерверы cleanvoice автоматически перезапускаются раз в сутки (утром) &mdash; для достижения максимальной стабильности на весь день.
</p>
<p>
<i>На планете сортировка делается по аптайму, поэтому при перезапуске сервер всегда оказывается внизу списка.</i>
</p>
<h3>Карты</h3>
<p>
Обновляются на серверах раз в минуту из <a href="https://github.com/NeedForKillTheGame/nfk-maps">https://github.com/NeedForKillTheGame/nfk-maps</a></small>
</p>

<p>
<h3>Как банить по IP</h3>
1. Узнать адрес нарушителя командой <b>svinfo</b>
<br>2. Если требуется забанить всю подсеть, то для конкретного IP адреса можно найти подсеть провайдера на <a href="http://www.whois-service.ru/lookup/">whois-service.ru</a> (см. CIDR или route). Задается она битовой маской (например, 12.34.56.0/20).
<br>3. Открыть файл <i>ipban.txt</i> и добавить ip (или подсеть) на новую строку. Можно добавить комментарий после знака решетки <i>#вот так</i>. При изменении на любом из серверов этот файл автоматически копируется на все остальные!
<br>4. Кикнуть игрока с сервера через консоль командой <b>kickplayer</b>. Теперь он не сможет зайти ни на один из серверов.
</p>
</div>
<br />
<div class='author'><a href="https://github.com/NeedForKillTheGame/nfk-service-launcher">Source code on Github</a></div>



<!-- console modal message -->
<div id="console-modal">

	<textarea id="clog" readonly>Initializing console...</textarea>
	<br>
	<input id="ctext" type="text" value="" />
	
</div>


<div id="file-modal" title="NFK Config">
	<div id="tabs">

		<ul>
		<?php foreach (Config::$Files as $i => $f): ?>
			<li><a href="#tabs-<?php echo $i; ?>"><?php echo $f; ?></a></li>
		<?php endforeach; ?>
		</ul>

		<?php foreach (Config::$Files as $i => $f): ?>
		<div id="tabs-<?php echo $i; ?>">
			<textarea id="ftext-<?php $i ?>">Loading config...</textarea>
		</div>
		<?php endforeach; ?>
		
		<div class="button-container">
			<button id="fsave">Save</button>
			<button id="fcancel">Cancel</button>
		</div>
		<div class="clear"></div>
	</div>
</div>


<svg>
  <line id="line" stroke-width="2px" stroke="red"  x1="0" y1="0" x2="100" y2="100"/>
</svg>


<style>
@keyframes fadeout {
   from    { opacity: 1;  }
   to      { opacity: 0; display: none; }
}

svg  {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100vh;
  animation: fadeout 0.4s both ease-in; 
  display: none;
  cursor: help;
}

#line_src {
	border-bottom: 1px dashed #333;
	margin-bottom: 3px;
    cursor: help;
}
</style>

<script language="javascript">

var instance_count = <?php echo count(Config::$Instances) ?>;
	
var line_el = document.getElementById("line");
var div1_el = document.getElementById("line_src");
var div2_el = document.getElementById("line_dst");

	
div1_el.addEventListener('mouseenter', e => {
	draw_line(e);
});


function draw_line(e)
{
	var x1 = e.x;
	//var x1 = div1_el.offsetLeft + (div1_el.offsetWidth / 2);
	var y1 = div1_el.offsetTop;
	var x2 = div2_el.offsetLeft + 50;
	var y2 = div2_el.offsetTop + (div2_el.offsetHeight);
	
	line_el.setAttribute('x1', x1);
	line_el.setAttribute('y1', y1);
	line_el.setAttribute('x2', x2);
	line_el.setAttribute('y2', y2);
	line_el.parentElement.style.display = "block";
	
	// hide after animation end
	setTimeout(function(){
		line_el.parentElement.style.display = "none";
	}, 900);
}


</script>


<link rel="stylesheet" href="//code.jquery.com/ui/1.10.0/themes/base/jquery-ui.css" />

<script type='text/javascript' src='//ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js'></script>
<script src="//code.jquery.com/ui/1.10.0/jquery-ui.min.js"></script>
<script type="text/javascript" src="js/noty/jquery.noty.js"></script>
<script type="text/javascript" src="js/noty/layouts/topRight.js"></script>
<script type="text/javascript" src="js/noty/themes/default.js"></script>

<script type='text/javascript' src='js/app.js'></script>
</body>
</html>
