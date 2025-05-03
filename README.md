# Vendendo a Fazenda Game

Bem-vindo ao "Vendendo a Fazenda"! Este é um jogo simples de combinação onde seu objetivo é limpar a grade de animais clicando nos botões correspondentes. Cada cor representa um tipo diferente de animal da fazenda: Cavalo (Branco), Cobra (Cinza), Galinha (Amarelo), Gato (Azul), Cachorro (Vermelho) e Vaca (Verde).

Este jogo foi desenvolvido como um projeto de computação gráfica utilizando C# e a biblioteca OpenTK, que fornece bindings para OpenGL. A lógica principal envolve a criação de uma grade de retângulos e botões de seleção. Inicialmente baseado em cores, o jogo foi aprimorado para mapear texturas de animais nos retângulos e botões correspondentes a cada cor. A biblioteca `StbImageSharp` é usada para carregar os arquivos de imagem (.jpg) e o OpenGL é utilizado para renderizar os retângulos texturizados na tela através de shaders (GLSL). A pontuação e a lógica de turnos adicionam um elemento de desafio.

## Gameplay

![Gameplay Demo](Textures/textures.gif?raw=true)

## Animais da Fazenda

| Animal   | Imagem na Grade                                       |
| :------- | :---------------------------------------------------- |
| Vaca     | <img src="Textures/cow.jpg?raw=true" width="50" alt="Vaca">       |
| Cachorro | <img src="Textures/dog.jpg?raw=true" width="50" alt="Cachorro">   |
| Gato     | <img src="Textures/cat.jpg?raw=true" width="50" alt="Gato">       |
| Galinha  | <img src="Textures/chicken.jpg?raw=true" width="50" alt="Galinha"> |
| Cavalo   | <img src="Textures/horse.jpg?raw=true" width="50" alt="Cavalo">   |
| Cobra    | <img src="Textures/snake.jpg?raw=true" width="50" alt="Cobra">    |

Clique em um botão de animal na parte inferior para remover todos os animais correspondentes da grade. O jogo termina quando todos os botões forem usados. Tente conseguir a maior pontuação (em "Reais")! Pressione 'R' se o jogo acabar para jogar novamente. Certifique-se de que a pasta `Textures` com as imagens dos animais (`animal.jpg`, `button-animal.jpg`) e a animação (`textures.gif`) esteja presente no diretório de execução.

Divirta-se!
