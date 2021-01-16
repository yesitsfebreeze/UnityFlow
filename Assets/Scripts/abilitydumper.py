import itertools



# liquids
# solids
# vapor

abilities = [
  "foam",
  "lubrication",
  "coolant"
  ]

duplets = 2
triplets = 3


print("")
print("")
print("")
print("------------------")
duplets = list(itertools.permutations(abilities, duplets))
print("duplets " + str(len(duplets)))
for duplet in duplets:
  print(duplet)

print("")
print("")
print("")
print("------------------")
triplets = list(itertools.permutations(abilities, triplets))
print("triplets " + str(len(triplets)))
for triplet in triplets:
  print(triplet)
