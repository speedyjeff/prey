## Predator v Prey
Predator (blue) versus prey (yellow) simulator.  This is inspired by [AI Battles](https://www.youtube.com/watch?v=qwrp3lB-jkQ).

![game2](https://github.com/speedyjeff/prey/blob/main/media/game2.gif)

The AI are driven by a neural networks that choose the direction and speed of movement.  In the initial configuration the predators (blue) hunt and attack the prey (yellow).  A series of experiments were run to guage the fitness of these networks (compared with a random movement AI).  The results show that the average lifetime of a predator is ___ higher than moving randomly.

### Basic Rules
The basic rules for the simulatio are that predators must eat prey to survive, and prey try to survive as long as they can.

![game3](https://github.com/speedyjeff/prey/blob/main/media/game3.gif)

Predators
 * Movement consume energy, standing still consumes energy
 * Must consume prey in order to gain energy and reproduce
 * Once energy is depleted, they will die
 * Touching a prey is lethal
 * Eating when not fully digested their previous meal will gain energy, but not contribute to their next reproduction
 * The field of vision is narrow and long

Prey 
 * Movement consumes energy, standing still replenishes energy
 * Reproductio happens as long as they stay alive long enough
 * Once energy is depleted, they cannot move
 * The field of vision is wide and short

Optional rules include allowing prey to fight back and kill predators, requiring N hits to deal a lethal hit, etc.

### Mechanics
The AI are governed by a set of meters that are adjusted for every update.  An update is designed to happen 10 times a second.  Predators and prey have specific costs/update for each of these meters.

 * Energy - needed to move/survive
 * Digestion - (predator) when still digesting, eating more prey does not lower the reproduction meter
 * Reproduction
   * (predator) adjusts when digesting prey
   * (prey) adjusted per update
 * Attack - a cool down between each attack

Each AI also has a Health meter, which is only adjusted if attacked by an AI of the opposite type.

The field of view is generated by extending rays out in front of the AI and returning the objects that are the closest.

![game1](https://github.com/speedyjeff/prey/blob/main/media/game1.gif)

Movement and speed are determined by two neural networks.  These neural networks are trained by actions that the AI take.  At first, the actions are largely random, but as the AI survives the actions are reinforced by actions that yield longer life.  New AI have a chance to inhert traits from their parents or the best surviving AI.


### Experiments
Experiments are run to track if neural network trained AI can have extended average lifetimes to random AI.  The termination conditions for the experiments are either of the populations are extinct, or a maximum lifetime is reached (maximum number of updates).

The minimum / maximum lifetimes: (without being killed)
 * Predator - 200 thru 1000 (even further if consuming prey)
 * Prey - maxium lifetime (per the basic rules, prey will stay still once energy is depleted)


#### Small experiment
The small experiments have a limit of ~1,000 AI.  This is a great experiment to visualize how the modes evolve and interact.

Configuration:
 * Width - 6,400
 * Height - 6,400
 * Predator (initial/max) - 50/170
 * Prey (initial/max) - 300/800
 * Combat - variable (peaceful or aggressive)
 * Random - variable
 * Iterations - 10
 * Max Lifetime - 5,000

```
./simulationcli -width 6400 -height 6400 -initialpred 50 -initialprey 300 -maxpred 170 -maxprey 800 -combat $0 -random $1 -it 10 -maxl 5000
```

##### Predator (peaceful) v Prey (peaceful)
This configuration is useful for a baseline, it will help determine local minimums for the predators as they require combat (eating) to survive.  The average lifetime of predator (neural) was 70% higher than predator (random).  The average lifetime for predators (neural) were 586-589 updates, where as predator (random) was 333-334 updates.

<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 673 | 0 | 800 | 672 | 0 | 1 | 0 | 421 | 0
1 | 698 | 0 | 800 | 696 | 1 | 0 | 0 | 606 | 0
2 | 675 | 0 | 800 | 672 | 2 | 0 | 0 | 589 | 0
3 | 689 | 0 | 800 | 688 | 3 | 0 | 0 | 616 | 0
4 | 680 | 0 | 800 | 680 | 4 | 0 | 0 | 560 | 0
5 | 710 | 0 | 800 | 708 | 5 | 0 | 0 | 618 | 0
6 | 671 | 0 | 800 | 669 | 6 | 0 | 0 | 626 | 0
7 | 712 | 0 | 800 | 710 | 7 | 0 | 0 | 609 | 0
8 | 676 | 0 | 800 | 676 | 8 | 1 | 0 | 611 | 0
9 | 703 | 0 | 800 | 702 | 9 | 0 | 0 | 606 | 0
###### Predator (random) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 359 | 0 | 800 | 359 | 0 | 0 | 0 | 334 | 0
1 | 362 | 0 | 800 | 360 | 1 | 0 | 0 | 338 | 0
2 | 355 | 0 | 800 | 352 | 2 | 1 | 0 | 334 | 0
3 | 365 | 0 | 800 | 364 | 3 | 1 | 0 | 335 | 0
4 | 364 | 0 | 800 | 364 | 4 | 1 | 1 | 334 | 0
5 | 354 | 0 | 800 | 353 | 5 | 0 | 0 | 331 | 0
6 | 370 | 0 | 800 | 370 | 0 | 0 | 0 | 335 | 0
7 | 355 | 0 | 800 | 355 | 1 | 1 | 0 | 332 | 0
8 | 360 | 0 | 800 | 359 | 2 | 0 | 0 | 335 | 0
9 | 365 | 0 | 800 | 363 | 3 | 0 | 0 | 335 | 0
###### Predator (neural) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 676 | 0 | 800 | 668 | 0 | 0 | 0 | 429 | 0
1 | 674 | 0 | 800 | 674 | 1 | 0 | 0 | 614 | 0
2 | 701 | 0 | 800 | 690 | 2 | 0 | 0 | 620 | 0
3 | 699 | 0 | 800 | 692 | 3 | 0 | 0 | 613 | 0
4 | 700 | 0 | 800 | 684 | 4 | 0 | 0 | 611 | 0
5 | 700 | 0 | 800 | 694 | 5 | 0 | 0 | 609 | 0
6 | 689 | 0 | 800 | 672 | 6 | 0 | 0 | 604 | 0
7 | 687 | 0 | 800 | 686 | 7 | 0 | 0 | 601 | 0
8 | 691 | 0 | 800 | 688 | 8 | 0 | 0 | 588 | 0
9 | 706 | 0 | 800 | 696 | 9 | 1 | 0 | 604 | 0
###### Predator (random) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 359 | 0 | 801 | 352 | 0 | 0 | 0 | 330 | 0
1 | 387 | 0 | 800 | 372 | 1 | 1 | 0 | 334 | 0
2 | 367 | 0 | 800 | 359 | 2 | 0 | 0 | 335 | 0
3 | 361 | 0 | 800 | 352 | 3 | 0 | 0 | 334 | 0
4 | 368 | 0 | 800 | 354 | 4 | 0 | 0 | 336 | 0
5 | 371 | 0 | 800 | 363 | 5 | 1 | 0 | 333 | 0
6 | 363 | 0 | 800 | 357 | 6 | 0 | 0 | 333 | 0
7 | 381 | 0 | 800 | 372 | 0 | 0 | 0 | 335 | 0
8 | 353 | 0 | 800 | 351 | 1 | 0 | 0 | 332 | 0
9 | 369 | 0 | 800 | 356 | 2 | 0 | 0 | 335 | 0
</details>

##### Predator (aggressive) v Prey (peaceful)
This is the default configuration, the predators hunt the prey. The predator lifetime saw a 40% increase when using the predator (neural) over predator (random).  The average lifetime of predator (neural) was 670-818 updates, where as predator (random) was 514-559 updates.  

<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 5000 | 170 | 800 | 2490 | 5 | 3288 | 2 | 396 | 238
1 | 5000 | 170 | 800 | 1912 | 11 | 4495 | 3 | 651 | 560
2 | 4086 | 0 | 800 | 1674 | 12 | 3363 | 5 | 841 | 610
3 | 4221 | 0 | 800 | 1689 | 14 | 3335 | 7 | 855 | 519
4 | 4597 | 0 | 800 | 1681 | 17 | 3956 | 8 | 842 | 826
5 | 5000 | 170 | 799 | 1788 | 18 | 3899 | 9 | 469 | 302
6 | 5000 | 166 | 800 | 2004 | 25 | 4306 | 11 | 486 | 383
7 | 3905 | 0 | 800 | 1414 | 26 | 3705 | 13 | 774 | 431
8 | 4235 | 0 | 800 | 1614 | 27 | 3211 | 14 | 860 | 585
9 | 5000 | 166 | 801 | 1551 | 8 | 4068 | 15 | 535 | 339
###### Predator (random) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 5000 | 170 | 797 | 2020 | 5 | 2542 | 9 | 545 | 251
1 | 5001 | 157 | 800 | 2302 | 10 | 2648 | 12 | 518 | 243
2 | 5001 | 169 | 790 | 2038 | 14 | 3212 | 6 | 512 | 247
3 | 5000 | 166 | 799 | 2305 | 23 | 3939 | 9 | 511 | 270
4 | 5000 | 169 | 800 | 2013 | 32 | 3409 | 11 | 512 | 259
5 | 5000 | 170 | 794 | 1608 | 39 | 3469 | 19 | 516 | 269
6 | 5000 | 170 | 797 | 1638 | 47 | 3365 | 23 | 494 | 283
7 | 5000 | 169 | 800 | 1904 | 54 | 3145 | 25 | 545 | 260
8 | 5001 | 168 | 799 | 1786 | 64 | 2932 | 26 | 492 | 285
9 | 5000 | 169 | 799 | 1762 | 72 | 3610 | 27 | 499 | 273
###### Predator (neural) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 5002 | 169 | 800 | 2278 | 1 | 4386 | 1 | 675 | 414
1 | 5000 | 170 | 800 | 1688 | 4 | 3116 | 7 | 630 | 321
2 | 5000 | 170 | 800 | 2314 | 8 | 3541 | 11 | 726 | 353
3 | 5003 | 170 | 800 | 2645 | 9 | 4481 | 1 | 943 | 542
4 | 5004 | 170 | 801 | 2337 | 13 | 4445 | 0 | 769 | 436
5 | 5001 | 169 | 799 | 2788 | 18 | 4267 | 4 | 919 | 519
6 | 5002 | 170 | 800 | 3136 | 19 | 4011 | 6 | 904 | 528
7 | 5005 | 167 | 800 | 3101 | 21 | 4517 | 10 | 922 | 522
8 | 5008 | 168 | 800 | 2834 | 23 | 4394 | 15 | 958 | 526
9 | 5002 | 168 | 799 | 2625 | 26 | 4093 | 18 | 740 | 336
###### Predator (random) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 5007 | 169 | 800 | 2371 | 3 | 2679 | 6 | 546 | 270
1 | 5005 | 170 | 797 | 2218 | 10 | 3076 | 9 | 534 | 273
2 | 5001 | 169 | 800 | 2279 | 11 | 3016 | 5 | 545 | 269
3 | 5001 | 170 | 800 | 2168 | 18 | 2712 | 7 | 562 | 263
4 | 5002 | 169 | 797 | 1927 | 23 | 3127 | 13 | 577 | 259
5 | 5000 | 170 | 800 | 1765 | 26 | 3090 | 20 | 557 | 272
6 | 5004 | 170 | 801 | 1996 | 27 | 2644 | 24 | 579 | 265
7 | 5002 | 169 | 797 | 2214 | 28 | 2904 | 0 | 549 | 274
8 | 5006 | 167 | 798 | 2096 | 31 | 2574 | 5 | 566 | 264
9 | 5000 | 170 | 799 | 2074 | 7 | 3229 | 8 | 583 | 256
</details>

##### Predator (peaceful) v Prey (aggressive)
This configuration has a double negative for predators, they are not able to reproduce (as they cannot eat) and prey are agressive.  There was a significant drop (2x) in average lifetimes for predators compared with the baseline.

<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 486 | 0 | 800 | 486 | 0 | 1 | 0 | 211 | 0
1 | 461 | 0 | 800 | 461 | 1 | 0 | 0 | 138 | 0
2 | 649 | 0 | 800 | 648 | 2 | 0 | 0 | 209 | 0
3 | 676 | 0 | 800 | 676 | 3 | 0 | 0 | 235 | 0
4 | 512 | 0 | 800 | 512 | 4 | 1 | 0 | 192 | 0
5 | 698 | 0 | 800 | 698 | 5 | 0 | 0 | 317 | 0
6 | 642 | 0 | 800 | 642 | 6 | 0 | 0 | 199 | 0
7 | 652 | 0 | 800 | 652 | 7 | 0 | 0 | 182 | 0
8 | 439 | 0 | 800 | 438 | 8 | 0 | 0 | 146 | 0
9 | 666 | 0 | 800 | 666 | 9 | 0 | 0 | 193 | 0
###### Predator (random) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 347 | 0 | 800 | 346 | 0 | 0 | 0 | 171 | 0
1 | 351 | 0 | 800 | 350 | 1 | 1 | 0 | 184 | 0
2 | 358 | 0 | 800 | 358 | 2 | 0 | 0 | 231 | 0
3 | 342 | 0 | 801 | 342 | 3 | 0 | 0 | 182 | 0
4 | 362 | 0 | 800 | 361 | 4 | 0 | 0 | 186 | 0
5 | 347 | 0 | 800 | 347 | 5 | 0 | 0 | 180 | 0
6 | 347 | 0 | 800 | 346 | 6 | 0 | 0 | 177 | 0
7 | 342 | 0 | 800 | 342 | 7 | 0 | 0 | 155 | 0
8 | 340 | 0 | 800 | 340 | 8 | 0 | 0 | 189 | 0
9 | 336 | 0 | 800 | 335 | 9 | 0 | 0 | 166 | 0
###### Predator (neural) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 380 | 0 | 800 | 376 | 0 | 0 | 0 | 161 | 0
1 | 310 | 0 | 801 | 308 | 1 | 1 | 2 | 143 | 0
2 | 612 | 0 | 800 | 604 | 2 | 0 | 0 | 177 | 0
3 | 413 | 0 | 800 | 406 | 3 | 1 | 2 | 144 | 0
4 | 521 | 0 | 800 | 519 | 4 | 1 | 3 | 174 | 0
5 | 395 | 0 | 800 | 394 | 5 | 0 | 0 | 170 | 0
6 | 692 | 0 | 800 | 691 | 6 | 2 | 2 | 172 | 0
7 | 607 | 0 | 800 | 602 | 0 | 1 | 3 | 165 | 0
8 | 523 | 0 | 800 | 514 | 1 | 0 | 0 | 182 | 0
9 | 540 | 0 | 800 | 539 | 2 | 0 | 0 | 151 | 0
###### Predator (random) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 341 | 0 | 800 | 337 | 0 | 1 | 2 | 150 | 0
1 | 349 | 0 | 800 | 344 | 1 | 0 | 0 | 183 | 0
2 | 353 | 0 | 800 | 348 | 2 | 0 | 0 | 181 | 0
3 | 327 | 0 | 800 | 322 | 3 | 0 | 0 | 172 | 0
4 | 352 | 0 | 800 | 345 | 4 | 0 | 0 | 162 | 0
5 | 322 | 0 | 800 | 320 | 5 | 0 | 0 | 144 | 0
6 | 349 | 0 | 800 | 339 | 6 | 1 | 0 | 176 | 0
7 | 354 | 0 | 800 | 349 | 7 | 0 | 0 | 185 | 0
8 | 336 | 0 | 800 | 331 | 8 | 0 | 0 | 165 | 0
9 | 335 | 0 | 800 | 331 | 9 | 0 | 0 | 177 | 0
</details>

##### Predator (aggresive) v Prey (aggressive)
This is the purge mode, everyone is attacking everyone.  The prey seem to have the advantage as they have the numbers and significantly reduce (5x) the length of the game, espeically once the prey (neural) model is trained.

<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 4543 | 0 | 800 | 809 | 2 | 4199 | 0 | 298 | 1618
1 | 5001 | 10 | 800 | 1055 | 16 | 4037 | 1 | 315 | 445
2 | 3621 | 0 | 800 | 1289 | 17 | 2737 | 1 | 336 | 416
3 | 913 | 0 | 800 | 486 | 18 | 865 | 2 | 172 | 169
4 | 649 | 0 | 800 | 648 | 0 | 331 | 3 | 193 | 105
5 | 717 | 0 | 800 | 716 | 1 | 517 | 4 | 260 | 102
6 | 726 | 0 | 800 | 726 | 2 | 408 | 5 | 240 | 88
7 | 729 | 0 | 800 | 729 | 3 | 345 | 7 | 254 | 81
8 | 770 | 0 | 800 | 770 | 0 | 326 | 8 | 248 | 98
9 | 702 | 0 | 800 | 702 | 1 | 470 | 10 | 227 | 104
###### Predator (random) v Prey (neural)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 4448 | 0 | 800 | 560 | 11 | 4127 | 0 | 284 | 1102
1 | 5000 | 48 | 800 | 912 | 32 | 3960 | 3 | 344 | 615
2 | 5000 | 14 | 800 | 794 | 38 | 3801 | 6 | 324 | 562
3 | 5000 | 17 | 800 | 822 | 56 | 4656 | 8 | 348 | 816
4 | 5000 | 100 | 800 | 800 | 60 | 4757 | 1 | 322 | 665
5 | 847 | 0 | 800 | 510 | 61 | 694 | 3 | 204 | 202
6 | 1772 | 0 | 800 | 422 | 65 | 1428 | 4 | 197 | 202
7 | 519 | 0 | 800 | 436 | 66 | 482 | 0 | 198 | 118
8 | 681 | 0 | 800 | 427 | 67 | 594 | 1 | 209 | 90
9 | 829 | 0 | 800 | 416 | 0 | 722 | 3 | 176 | 232
###### Predator (neural) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 5000 | 29 | 800 | 549 | 4 | 4977 | 0 | 203 | 1577
1 | 720 | 0 | 800 | 480 | 5 | 669 | 1 | 193 | 188
2 | 713 | 0 | 801 | 475 | 6 | 596 | 2 | 205 | 183
3 | 847 | 0 | 801 | 430 | 7 | 653 | 4 | 200 | 149
4 | 730 | 0 | 800 | 438 | 8 | 572 | 6 | 231 | 162
5 | 661 | 0 | 800 | 448 | 9 | 470 | 7 | 221 | 137
6 | 538 | 0 | 800 | 468 | 0 | 435 | 8 | 195 | 135
7 | 780 | 0 | 800 | 465 | 2 | 704 | 0 | 235 | 183
8 | 1837 | 0 | 802 | 436 | 3 | 1816 | 1 | 211 | 495
9 | 866 | 0 | 800 | 406 | 4 | 591 | 3 | 233 | 181
###### Predator (random) v Prey (random)
Iteration | Lifetime | AlivePredators | AlivePrey | BestPredatorLifetime | BestPredatorGeneration | BestPreyLifetime | BestPreyGenertation | AvgPredatorLifetime | AvgPreyLifetime
----------|----------|----------------|-----------|----------------------|------------------------|------------------|----------------------|---------------------|----------------
0 | 1019 | 0 | 800 | 457 | 1 | 869 | 1 | 216 | 155
1 | 2039 | 0 | 800 | 508 | 6 | 1895 | 2 | 222 | 395
2 | 875 | 0 | 800 | 472 | 7 | 697 | 4 | 231 | 184
3 | 1221 | 0 | 800 | 420 | 8 | 1114 | 5 | 214 | 252
4 | 1680 | 0 | 800 | 526 | 12 | 1463 | 0 | 233 | 219
5 | 930 | 0 | 800 | 401 | 0 | 870 | 0 | 221 | 166
6 | 635 | 0 | 800 | 432 | 1 | 598 | 1 | 228 | 147
7 | 1778 | 0 | 800 | 467 | 2 | 1558 | 3 | 220 | 363
8 | 526 | 0 | 800 | 471 | 4 | 394 | 5 | 224 | 136
9 | 1238 | 0 | 800 | 508 | 0 | 1046 | 6 | 245 | 233
</details>



#### Large experiment
The large experiments have a limit of ~16,000 AI, an increase of 16x over the small experiment.

Configuration:
 * Width - 16,000
 * Height - 16,000
 * Predator (initial/max) - 800/800
 * Prey (initial/max) - 4800/13000
 * Combat - variable (peaceful or aggressive)
 * Random - variable
 * Iterations - 10
 * Max Lifetime - 10,000

```
./simulationcli -width 16000 -height 16000 -initialpred 800 -initialprey 4800 -maxpred 800 -maxprey 13000 -combat $0 -random $1 -it 10 -maxl 10000 
```






##### Predator (peaceful) v Prey (peaceful)
<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
###### Predator (random) v Prey (neural)
###### Predator (neural) v Prey (random)
###### Predator (random) v Prey (random)
</details>

##### Predator (aggressive) v Prey (peaceful)
<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
###### Predator (random) v Prey (neural)
###### Predator (neural) v Prey (random)
###### Predator (random) v Prey (random)
</details>

##### Predator (peaceful) v Prey (aggressive)
<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
###### Predator (random) v Prey (neural)
###### Predator (neural) v Prey (random)
###### Predator (random) v Prey (random)
</details>

##### Predator (aggresive) v Prey (aggressive)
<details>
<summary>data</summary>
###### Predator (neural) v Prey (neural)
###### Predator (random) v Prey (neural)
###### Predator (neural) v Prey (random)
###### Predator (random) v Prey (random)
</details>



### Next steps
There is a lot more tuning possible in this experiment.  A few areas of focus would be to nerf prey attacks (as they currently are like a hord of unstoppable zobmies and overwhelm the predators), and further tuning of AI meters.

### Trying
This project is build able with Visual Studio 2022 or later.  Below are the steps to reproduce these experiments.

Visual representation (Windows only)
```
simulation.exe
```

Cross platform and on the command line.
```
./simulationcli.exe
```

This is all the available options for trying out the simulation from the command line.
```
./simulationcli.exe -help
./simulationcli
  -(help)             : this help
  -(w)idth            : the 2d width of the environment (default: 6400)
  -(hei)ght           : the 2d height of the enviorment (default: 6400)
  -(initialPred)ators : the initial number of predators at start (default: 50)
  -(maxPred)ators     : the max number of predators (default: 170)
  -(initialPrey)      : the initial number of prey at start (default: 300)
  -(maxPrey)          : the max numbre of prey (default: 800)
  -(c)ombat           : allowed combat (0: none, 1:Predators, 2:Prey, 3:Predators+Prey) (default: 1)
  -(r)andom           : random movement (0: none, 1:Predators, 2:Prey, 3:Predators+Prey) (default: 0)
  -(it)erations       : number of training runs (a run stops when either Predators or Prey are extinct) (default: 10)
  -(maxl)ifetime      : maximum number of updates per iteration (default: 5000
```

### Initialization
git submodule init
git submodule update
