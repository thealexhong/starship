filename = '18 Evgeny Morning_apostropied'

with open(filename + '.csv') as f:
	lines = f.read().splitlines()

new_f = open(filename + "_UNapostropied.csv", 'wb')
apostrophied_lines = [None]*len(lines)

for i in range(len(lines)):
	col = lines[i].split(",")
	col[14] = col[14][1:]
	#print col
	apostrophied_lines[i] = ','.join(map(str, col))
	print apostrophied_lines[i]
	new_f.write(apostrophied_lines[i] + "\n")

new_f.close()