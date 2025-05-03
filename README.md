# Vendendo a Fazenda Game

Bem-vindo ao "Vendendo a Fazenda"! Este é um jogo simples de combinação onde seu objetivo é limpar a grade de animais clicando nos botões correspondentes. Cada cor representa um tipo diferente de animal da fazenda: Cavalo (Branco), Cobra (Cinza), Galinha (Amarelo), Gato (Azul), Cachorro (Vermelho) e Vaca (Verde).

Este jogo foi desenvolvido como um projeto de computação gráfica utilizando C# e a biblioteca OpenTK, que fornece bindings para OpenGL. A lógica principal envolve a criação de uma grade de retângulos e botões de seleção. texturas de animais foram mapeadas nos retângulos e botões correspondentes a cada cor e animal. A biblioteca `StbImageSharp` é usada para carregar os arquivos de imagem (.jpg) e o OpenGL é utilizado para renderizar os retângulos texturizados na tela através de shaders (GLSL). A pontuação e a lógica de turnos adicionam um elemento de desafio.

## Animais da Fazenda

| Animal   | Imagem na Grade                 |
| :------- | :------------------------------ |
| Vaca     | ![Vaca](Textures/cow.jpg?raw=true)     |
| Cachorro | ![Cachorro](Textures/dog.jpg?raw=true) |
| Gato     | ![Gato](Textures/cat.jpg?raw=true)     |
| Galinha  | ![Galinha](Textures/chicken.jpg?raw=true)  |
| Cavalo   | ![Cavalo](Textures/horse.jpg?raw=true)   |
| Cobra    | ![Cobra](Textures/snake.jpg?raw=true)    |

Clique em um botão de animal na parte inferior para remover todos os animais correspondentes da grade. O jogo termina quando todos os botões forem usados. Tente conseguir a maior pontuação (em "Reais")! Pressione 'R' se o jogo acabar para jogar novamente. Certifique-se de que a pasta `Textures` com as imagens dos animais (`animal.jpg` e `button-animal.jpg`) esteja presente no diretório de execução.

Divirta-se!
