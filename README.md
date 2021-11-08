# Unity Space Invaders

### Juego realizado utilizando Unity Version 2019.4.18f1 compilado para Windows con IL2CPP.
Lenguaje utilizado: C#

Se realizo una grilla con los enemigos, para moverlos en conjunto y detectar los relacionados
por el "tipo" de enemigo, representado mediante ScriptableObjects, lo que permite que a futuro
estos datos sean cargados desde objetos JSON
Juego realizado en 2D sobre el template de URP, para poder utilizar shaders personalizados
y generar efecto de textura rota sobre los enemigos con mas de 1 vida

Asset utilizado de la store: Auto LetterBox (Se encarga de mantener la relacion de aspecto
independientemente de la pantalla donde corra el juego)
Fueron eliminados los metodos que no se utilizaban en el proyecto.

### Se generaron dos assets en los sprites 
Un sprite cuadrado simple generado desde la misma herramienta Unity utilizado para las barreras.
Un sprite generado con Adobe Photoshop 2021, exportandolo como png de 8bits, para menor tamaño,
el tamaño de este sprite es de 128 x 16 para que unity pueda interpretarlo más rapido por ser potencia de 2.
La disminucion de tamaño con respecto al sprite original es minima, pero en otros casos puede ser considerable.