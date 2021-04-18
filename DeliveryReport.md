# Oizys - AI Para Simplexity

## Autores

### Afonso Lage

* Stuff

### André Santos

* Stuff

### Nelson Milheiro

* Stuff

## Descrição da Inteligência Artificial

### Algoritmo de Procura

Negamax com cortes Alfa-Beta

### Heurística Usada

1º - Inicializa as variáveis _score_ (pontuação da heurística) e _sequenceToWin_ (numero de peças em sequencia necessárias para ganhar).

2º - Itera todos os "corredores de vitória existentes".

3º - Inicializa as variáveis _sequence_ (numero de peças do jogador em sequencia) e _range_ (numero de peças seguidas incluindo espaços vazios) a 0.

4º - Itera todas as posições existentes nos corredores supramencionados.

5º - Guarda a posição atual e aumenta o _range_ por 1.

6º - Verifica se a posição tem uma peça, caso não tenha passa para a próxima posição e volta ao __passo 5__. Caso a posição tenha uma peça, vai para o __passo 7__.

7º - Verifica se a peça atual tem a mesma forma e cor que as peças do jogador. Caso tenha, aumenta o _score_ por 2, aumenta a _sequence_ por 1 e vai para o __passo 11__. Caso contrário, passa para o __passo 8__.

8º - Verifica se a peça atual tem a mesma forma, mas cor diferente às peças do jogador. Caso tenha, aumenta o _score_ por 1 e vai para o __passo 11__. Caso contrário vai para o __passo 9__.

9º - Verifica se a peça atual tem uma forma diferente, mas a mesma cor que as peças do jogador. Caso tenha, diminui o _score_ por 1 e vai para o __passo 11__. Caso contrário, vai para o __passo 10__.

10º - Verifica se a peça atual não tem a mesma forma, nem a mesma cor que as peças do jogador. Caso não tenha, diminui o _score_ por 2, verifica se o _range_ é menor que o _sequenceToWin_ e caso seja mete _sequence_ a 0 e por fim mete _range_ a 0 e vai para o __passo 11__.

11º - Verifica se o _range_ é igual à _sequenceToWin_. Caso isto seja verdade:

* Verifica se a _sequence_ é igual à _sequenceToWin_ - 1. Caso isto seja verdade, aumenta o _score_ por 10 e passa para o __passo 13__. Caso contrário, passa para o __seguinte ponto__.

* Verifica se a _sequence_ é igual à _sequenceToWin_ - 2. Caso isto se verifique, aumenta o _score_ por 5 e vai para o __passo final__.

12º - Retorna o _score_.

## Referências

Exemplo de IA para o TicTacToe, neste [repositório] com autoria de [Nuno Fachada].

[repositório]:https://github.com/fakenmc/AIUnityExamples
[Nuno Fachada]:https://github.com/fakenmc
