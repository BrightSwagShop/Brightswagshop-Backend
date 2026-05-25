import com.intuit.karate.junit5.Karate;

class FrontendRunner {
    @Karate.Test
    Karate testAll() {
        return Karate.run().relativeTo(getClass());
    }
}
