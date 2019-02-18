let fact x = 
    if x < 0 then
        0
    else
        let rec acc_fact x acc =
            if x <= 1 then
                acc
            else
                acc_fact (x-1) acc*(x-1)
        acc_fact x x
