# Timeless Emulator

A Path of Exile Timeless Jewel emulator.

## How Timeless Jewels modify passive skills

For each passive skill in range of the Timeless Jewel the game runs a function. Let's call this function `process_passive_skill`. 
The purpose of this function is to take a couple of inputs related to the processed passive skill and the socketed Timeless Jewel and output a modified passive skill.   
The inputs to this function include:

1. The processed passive skill's graph identifier (`graph_id`)
2. The type of the Timeless Jewel (`jewel_type`)
3. The transformed¹ Timeless Jewel's seed (`jewel_seed`)
4. The Timeless Jewel's conqueror and conqueror version² (`conqueror` and `conqueror_version`)

¹ The jewel seed is stored as a 2 byte unsigned integer, which means its maximum value is 2^16 - 1 = 65535. Elegant Hubris' maximum seed however is 160 000. This is actually "fake". The real seed range is 100 - 8000 which is then multiplied by 20 for displaying purposes. This means we have to divide the user's input seed by 20 to get the number we need for the calculations.  

² Each Timeless Jewel has a conqueror and each conqueror has a version number attached to them.  
When a conqueror is replaced, the index stays the same, but the version number is increased.    

#### Part 1 | Determination

First we need to determine if we need to fully replace the original passive skill (Elegant Hubris, Glorious Vanity and in some cases Militant Faith), if we just need to make additions to the original passive skill (Lethal Pride, Brutal Restraint and in some cases Militant Faith) or if we need to do both (looking at you, Glorious Vanity). 
Here's how that works:

1. If the processed passive skill is a keystone we always need to replace it.
2. If the processed passive skill is a notable we roll a random number between 0 and 100 and check if that number is below the jewel type's notable replacement spawn weight. Currently only Militant Faith uses conditional notable replacements (with a notable replacement spawn weight of 20). The notable replacement spawn weight for the other jewel types is either 0 or 100, which means they never or always replace notables. 
3. If the processed passive skill is a small attribute passive skill (those that give 10 strength, dexterity or intelligence) we check the data sheet if this jewel type replaces passive skills of that type.
4. If the processed passive skill is a small normal passive skill (all other small passives skills that aren't small attribute passive skills) we also check the data sheet if the jewel type replaces passive skills of that type.

#### Part 2 | Replacement

If we came to the conclusion that we have to replace the passive skill this is how it's done:

1. If the processed passive skill is a keystone, we search the replacement keystone in the data sheet using the conqueror and conqueror version inputs. We can skip the rest of the steps.
2. First we create a filtered list of all applicable alternate passive skills that could potentially replace the processed passive skill. This includes checking if certain properties like the Timeless Jewel's type matches the one attached to the alternate passive skill and so on. Next we iterate trough all applicable passive skills and roll a number between zero and the total spawn weight of all applicable alternate passive skills. If the roll is below the current iterated applicable passive skill's spawn weight we set this applicable passive skill as our temporary candidate for replacement, however we still iterate through the whole filtered list and replace the temporary candidate if the new roll is lower again.
3. Next we check how many stats are associated with the rolled passive skill replacement and roll the stats for each of them. This is as simple as rolling a random number between the minimum and maximum roll.

#### Part 3 | Addition

After part 2, or if we skipped part 2, we make additions to the processed passive skill. An example would be adding the "+5 Devotion" from Militant Faith to passive skills.  
A little note first. An addition is not the same as a Stat. In theory one addition can have multiple stats associated with them, however in practice every addition has only one Stat assigned. Addition works quite similar to replacement:

1. Determine how many additions have to be made. Lethal Pride, Brutal Restraint and Militant Faith Timeless Jewels all make only one addition. Our good friend Glorious Vanity can randomly make between three and four additions on certain notables. More on Glorious Vanity's shenanigans in the notes section.
2. Create a filtered list of applicable additions (similar conditions as in the replacement part). For each addition we have to make, roll a value between zero and the total spawn weight of all applicable additions. Iterate all applicable additions and check if the iterated addition's spawn weight is bigger than the roll. If it is, we have found the addition we'll add, if not subtract the iterated addition's spawn weight from the roll and continue iterating.
3. This step is the same as step 3 from the replacement part. We check how many stats are associated with the addition and roll each of them.

## The Pseudo Random Number Generator

The game uses a modified version of the [TinyMT32](https://datatracker.ietf.org/doc/html/draft-ietf-tsvwg-tinymt32) pseudo random number generator for all things related to the Timeless Jewel mechanic. The game's version has a different internal state size, a modified `tinymt32_next_state` function and uses the parameters `mat1`, `mat2` and `tmat` a bit differently than the original algorithm.  

The seed used for the pseudo random number generator is an array of two 4 byte unsigned integers.
The first is the `graph_id` and the second is the `jewel_seed` (cast to a 4 byte value).

## Notes

#### Glorious Vanity

Figuring this one out was a bit of a pain in the butt.  
Not only can this jewel replace passive skills, it can also add to passive skills.
But this isn't really obvious.  
There are two notables that can come out as a result of replacement, that have no stats associated with them:

- Legacy of the Vaal
- Might of the Vaal

When a replacement has happened and one of them was the outcome it then also runs the addition part from above on them.

#### What happens when the same addition is rolled twice?

This effectively can only happen on the two Glorious Vanity notables *Legacy of the Vaal* and *Might of the Vaal*.  
The answer is: they stack.

#### Does the Timeless Jewel's seed and its conqueror interact?

No, the seed only affects small and notable passive skills and the conqueror only keystones.  
This means two Timeless Jewels with the same seed but different conquerors affect small and notable passive skills the same way.