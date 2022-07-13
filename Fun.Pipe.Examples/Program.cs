using Fun.Pipe.Examples;

//await ExampleOpt.Run();
//ExampleRes.Run();
//await ExampleResT.Run();
//ExamplePipeParse.Run();     // pipe examples will lead to different result in each run
//ExamplePipeWebReq.Run();    // due to randomization to simulate different cases.



static Res<double> Mult(Res<int> num)
{
    if (num.IsErr)
        return ErrFrom<int, double>(num);
    else
        return Ok(12.0 * num.Unwrap());
}

Console.WriteLine(Mult(Ok(12)));
Console.WriteLine(Mult(Err<int>("nooo")));
Console.WriteLine();
