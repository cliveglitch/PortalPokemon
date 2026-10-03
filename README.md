# Prueba Tecnica Portal Pokemon

Aplicación web en .NET Core 8 MVC que muestra un listado de Pokémon's mediante el consumo de la API https://pokeapi.co/
<img width="1919" height="1026" alt="image" src="https://github.com/user-attachments/assets/073191c4-0f5f-4bb1-aa5b-e42811d434c5" />

## Funcionalidades

Se muestra un grid de máximo 30 Pokémon por pagina, cada Pokémon esta contenido en una carta con su sprite y nombre. La vista maneja varias páginas y el usuario puede navegar entre ellas mediante los controles localizados abajo del grid. Se puede dar click a un Pokémon para abrir una vista que muestra a detalle su información y también datos de su especie.

<img width="1919" height="1025" alt="image" src="https://github.com/user-attachments/assets/4d5c7394-55b8-4344-a1a6-3a34a7566cb1" />

El grid puede ser filtrado en la caja de texto por nombres de Pokémon y también por el dropdown de especies, los dos filtros localizados arriba del grid. 

Las requests hechas en la aplicación maneja cache para no tener que llamar de mas a la API de Pokémon, y también haciendo la experiencia de usuario mas rápida.

Se puede exportar la pagina actual del grid filtrado a un excel, que mostrará dos columnas, una con el nombre del Pokémon y la otra con su sprite. 

En la misma fila de los filtros al otro lado extremo hay una caja de texto que te permite entrar un correo electrónico que va a recibir la pagina actual del grid filtrado. El la vista que muestra la información de un Pokémon, en el mismo lugar, también tiene una caja de texto para recibir un correo, pero este enviará el detalle de ese Pokémon.
<img width="1919" height="1020" alt="image" src="https://github.com/user-attachments/assets/6accd2a7-cd2d-4a6c-9544-fc6aafe32bae" />

## Diseño de la aplicación

Se utilizó la interfaz IMemoryCache con el paquete Microsoft.Extensions.Caching.Memory para manejar cache en la memoria de la aplicación por ser simple y fácil de implementar, algo como HybridCache puede manejar requests simultaneous al mismo tiempo pero no se espera tener tantos usuarios.

Se utilizó el paquete ClosedXML para exportar el Excel de la pagina actual por si simpleza para ser implementado y su una licencia de uso libre, a diferencia de otras alternativas como EPPlus que requiere configuración para usarlo o Open-XML-SDK que requiere mas código para poder hacer la misma tarea mejor performance, pero con lo poco que se va a exportar esas ganancias no valen la pena.

Solo se configuró un timeout de 15 segundos porque específicamente las request que se hacen no son grandes ni lo suficiente complejas como para tener que implementar un deadline a una request de esta aplicación.

## Configuración de envío de correos

Para poder probarlo busca y abre el archivo appsettings.json y en el objeto Smtp puedes llenar la información necesaria, dejar la propiedad "UseAuthentication" en false si se va a usar un servidor de prueba como smtp4dev que no requiere autenticación.

Si no se va a usar un servidor de prueba puede cambiar la propiedad "UseAuthentication" a true y poner las credenciales de usuario y contraseña en un secrets.json que se puede encontrar en Visual Studio al darle click derecho al proyecto PortalPokemon y luego a la opción "Administrar secretos de usuario" que va a abrir el antes mencionado archivo, ahi ya puedes agregar el siguiente objeto llenando sus propiedades con tus credenciales:

"Smtp": {
  "Username": "",
  "Password": ""
}
